using System.Data.Entity;
using System.Globalization;
using feat.common;
using feat.common.Models;
using feat.common.Models.Enums;
using feat.common.Models.Staging.FAA.Enums;
using feat.ingestion.Configuration;
using feat.ingestion.Data;
using feat.ingestion.Enums;
using feat.ingestion.Models.FAA;
using NetTopologySuite.Geometries;
using Database = feat.common.Models.Staging.FAA;
using EntityFrameworkQueryableExtensions = Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions;
using Location = feat.common.Models.Location;

namespace feat.ingestion.Handlers.FAA;

public class FaaIngestionHandler(
    IngestionOptions options,
    IApiClient apiClient,
    IngestionDbContext dbContext)
    : IngestionHandler(options)
{
    private readonly IngestionOptions _options = options;

    public override IngestionType IngestionType => IngestionType.Api | IngestionType.Automatic;
    public override string Name => "Find An Apprenticeship";
    public override string Description => "Ingestion from Find An Apprenticeship API";

    public override async Task<bool> ValidateAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(_options.ApprenticeshipApiKey))
        {
            Console.WriteLine("Find An Apprenticeship API key not set.");
            return false;
        }

        const string url = "vacancies/vacancy?PageNumber=1&PageSize=1";
        ApiResponse response;

        try
        {
            response = await apiClient.GetAsync<ApiResponse>(
                ApiClientNames.FindAnApprenticeship, url, cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return false;
        }

        if (response.Total == 0)
        {
            Console.WriteLine("No results returned from Find An Apprenticeship API.");
            return false;
        }

        return true;
    }

    public override async Task<bool> IngestAsync(CancellationToken cancellationToken)
    {
        const int apiPageSize = 500;

        try
        {
            var allApprenticeships = new List<Database.Apprenticeship>();
            var allAddresses = new List<Database.Address>();
            var pageNumber = 1;

            while (true)
            {
                Console.WriteLine($"Fetching page {pageNumber} (PageSize={apiPageSize})...");
                
                var url = $"vacancies/vacancy?PageNumber={pageNumber}&PageSize={apiPageSize}";
                
                var response = await apiClient.GetAsync<ApiResponse>(
                    ApiClientNames.FindAnApprenticeship, url, cancellationToken: cancellationToken);

                var apprenticeships = response.Vacancies.FromDtoList();

                if (apprenticeships.Count == 0)
                {
                    Console.WriteLine($"No more apprenticeships found after page {pageNumber}. Stopping pagination.");
                    break;
                }

                allApprenticeships.AddRange(apprenticeships);
   
                Console.WriteLine($"Fetched {apprenticeships.Count} apprenticeships from page {pageNumber}.");

                if (apprenticeships.Count < apiPageSize)
                {
                    Console.WriteLine($"Last page detected (page {pageNumber}).");
                    break;
                }

                pageNumber++;
            }

            if (allApprenticeships.Count == 0)
            {
                Console.WriteLine("No apprenticeship data retrieved from the API.");
                return false;
            }
            
            Console.WriteLine($"Fetched {allApprenticeships.Count} total records from API.");
            
            var dedupedApprenticeships = allApprenticeships
                .GroupBy(a => a.VacancyReference)
                .Select(g => g.OrderByDescending(a => a.PostedDate).First())
                .ToList();
            
            var duplicateCount = allApprenticeships.Count - dedupedApprenticeships.Count;
            
            Console.WriteLine($"Removed {duplicateCount} duplicate records.");
            Console.WriteLine($"Syncing {dedupedApprenticeships.Count} deduped apprenticeship records to DB...");
            
            await dbContext.BulkSynchronizeAsync(dedupedApprenticeships, options =>
            {
                options.ColumnPrimaryKeyExpression = apprenticeship => apprenticeship.VacancyReference;
            }, cancellationToken);
            
            foreach (var apprenticeship in dedupedApprenticeships)
            {
                if (apprenticeship.Addresses.Count == 0)
                {
                    continue;
                }

                foreach (var address in apprenticeship.Addresses)
                {
                    address.ApprenticeshipId = apprenticeship.Id;
                    allAddresses.Add(address);
                }
            }
            
            Console.WriteLine($"Syncing {allAddresses.Count} address records to DB...");

            if (allAddresses.Count > 0)
            {
                await dbContext.BulkSynchronizeAsync(allAddresses, options =>
                {
                    options.ColumnPrimaryKeyExpression = a => new
                    {
                        a.ApprenticeshipId,
                        a.Postcode,
                        a.AddressLine1
                    };
                }, cancellationToken);
            }
            
            Console.WriteLine($"Find An Apprenticeship staging ingestion complete.");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to ingest Find An Apprenticeship data: {ex.Message}");
            return false;
        }
    }
    
    public override async Task<bool> Sync(CancellationToken cancellationToken)
    {
        var apprenticeships = EntityFrameworkQueryableExtensions.Include(
                dbContext.FAA_Apprenticeships.Include(a => a.Addresses),
                apprenticeship => apprenticeship.Addresses)
            .ToList();

        if (apprenticeships.Count == 0)
        {
            Console.WriteLine("No data found in staging tables.");
            return false;
        }

        Console.WriteLine($"Processing {apprenticeships.Count} apprenticeships from staging...");

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        
        // PROVIDERS
        
        var providers = apprenticeships
            .GroupBy(a => a.Ukprn)
            .Select(a => new Provider
        {
            Created = DateTime.UtcNow,
            Name = a.First().ProviderName ?? "Unknown provider",
            Ukprn = a.Key.ToString()
        }).ToList();
        
        await dbContext.BulkSynchronizeAsync(providers, options =>
        {
            options.IgnoreOnSynchronizeUpdateExpression = p => new
            {
                p.Id,
                p.Created
            };
            options.ColumnPrimaryKeyExpression = p => p.Ukprn;
        }, cancellationToken);

        var providerLookup = dbContext.Set<Provider>()
            .ToDictionary(p => p.Ukprn!, p => p.Id);

        Console.WriteLine($"Providers synchronized: {providers.Count}");
        
        // ENTRY
        
        var entries = apprenticeships.Select(a =>
        {
            var providerId = providerLookup[a.Ukprn.ToString()];
            return new Entry
            {
                Created = DateTime.UtcNow,
                ProviderId = providerId,
                Reference = a.VacancyReference,
                Title = a.Title,
                Description = a.Description,
                FlexibleStart = a.StartDate == null,
                AttendancePattern = MapCourseHours(a.HoursPerWeek),
                Url = a.ApplicationUrl ?? string.Empty,
                SourceSystem = SourceSystem.FAA,
                Type = EntryType.Apprenticeship,
                Level = StudyTime.Daytime // TODO: Check
            };
        }).ToList();
        
        await dbContext.BulkSynchronizeAsync(entries, options =>
        {
            options.IgnoreOnSynchronizeUpdateExpression = e => new
            {
                e.Id,
                e.Created
            };
            options.ColumnPrimaryKeyExpression = e => e.Reference;
        }, cancellationToken);

        var entryLookup = dbContext.Set<Entry>()
            .ToDictionary(e => e.Reference, e => e.Id);

        Console.WriteLine($"Entries synchronized: {entries.Count}");
        
        // ENTRYINSTANCE
        
        var entryInstances = apprenticeships.Select(a =>
        {
            var entryId = entryLookup[a.VacancyReference!];
            return new EntryInstance
            {
                Created = DateTime.UtcNow,
                EntryId = entryId,
                StartDate = a.StartDate,
                Duration = ParseMonthStringToTimeSpan(a.ExpectedDuration),
                StudyMode = LearningMethod.Workbased,
                Reference = a.VacancyReference
            };
        }).ToList();
        
        await dbContext.BulkSynchronizeAsync(entryInstances, options =>
        {
            options.IgnoreOnSynchronizeUpdateExpression = ei => new
            {
                ei.Id,
                ei.Created
            };
            options.ColumnPrimaryKeyExpression = ei => ei.Reference;
        }, cancellationToken);
        
        Console.WriteLine($"EntryInstances synchronized: {entries.Count}");
        
        // EMPLOYERS
        
        var employers = apprenticeships
            .GroupBy(a => a.EmployerName!.ToLower().Trim())
            .Select(a => new Employer
        {
            Created = DateTime.UtcNow,
            Name = a.Key,
            Url = a.First().EmployerWebsiteUrl,
            ContactName = a.First().EmployerContactName,
            ContactEmail = a.First().EmployerContactEmail,
            ContactPhone = a.First().EmployerContactPhone
        }).ToList();

        await dbContext.BulkSynchronizeAsync(employers, options =>
        {
            options.IgnoreOnSynchronizeUpdateExpression = e => new
            {
                e.Id,
                e.Created
            };
            options.ColumnPrimaryKeyExpression = e => e.Name;
        }, cancellationToken);

        var employerLookup = dbContext.Set<Employer>()
            .ToDictionary(e => e.Name.ToLower().Trim(), e => e.Id);

        Console.WriteLine($"Employers synchronized: {employers.Count}");

        // VACANCIES
        
        var vacancies = apprenticeships.Select(a =>
        {
            var entryId = entryLookup[a.VacancyReference!];
            var employerId = employerLookup[a.EmployerName!.ToLower().Trim()];
            return new Vacancy
            {
                EntryId = entryId,
                EmployerId = employerId,
                Level = MapApprenticeshipLevel(a.ApprenticeshipLevel),
                Positions = (short?)a.NumberOfPositions,
                Wage = (decimal?)a.WageAmount ?? ParseWageAmount(a.WageAdditionalInformation),
                WageUnit = MapWageUnit(a.WageUnit),
                WageType = Enum.Parse<WageType>(a.WageType),
                HoursPerWeek = a.HoursPerWeek,
                WorkingWeekDescription = a.WorkingWeekDescription,
                ClosingDate = a.ClosingDate,
                PostedDate = a.PostedDate,
                Url = a.VacancyUrl,
                NationalVacancy = a.IsNationalVacancy,
                NationalVacancyDetails = a.IsNationalVacancyDetails
            };
        }).ToList();
        
        await dbContext.BulkSynchronizeAsync(vacancies, options =>
        {
            options.IgnoreOnSynchronizeUpdateExpression = v => v.Id;
            options.ColumnPrimaryKeyExpression = v => v.EntryId;
        }, cancellationToken);

        Console.WriteLine($"Vacancies synchronized: {vacancies.Count}");
        
        // LOCATIONS
        
        var locations = apprenticeships
            .SelectMany(a => a.Addresses)
            .GroupBy(a => new
            {
                Postcode = a.Postcode!.ToLower().Trim(),
                AddressLine1 = a.AddressLine1?.ToLower().Trim() ?? string.Empty,
                AddressLine4 = a.AddressLine4?.ToLower().Trim() ?? string.Empty
            })
            .Select(a =>
            {
                var longitude = a.First().Longitude;
                var latitude = a.First().Latitude;
                
                return new Location
                {
                    Created = DateTime.UtcNow,
                    Address1 = a.Key.AddressLine1,
                    Address2 = a.First().AddressLine2,
                    Address3 = a.First().AddressLine3,
                    Address4 = a.First().AddressLine4,
                    Postcode = a.Key.Postcode,
                    GeoLocation = longitude != null && latitude != null
                        ? new Point(longitude.Value, latitude.Value) { SRID = 4326 }
                        : null
                };
            }).ToList();

        await dbContext.BulkSynchronizeAsync(locations, options =>
        {
            options.IgnoreOnSynchronizeUpdateExpression = l => new
            {
                l.Id,
                l.Created
            };
            options.ColumnPrimaryKeyExpression = l => new
            {
                l.Postcode,
                l.Address1,
                l.Address4
            };
        }, cancellationToken);
        
        Console.WriteLine($"Locations synchronized: {locations.Count}");
        
        await transaction.CommitAsync(cancellationToken);
        Console.WriteLine("Find An Apprenticeship table sync complete.");

        return true;
    }
    
    private static CourseHours? MapCourseHours(decimal? hoursPerWeek)
    {
        return hoursPerWeek switch
        {
            null => null,
            < 35 => CourseHours.PartTime,
            _ => CourseHours.FullTime
        };
    }
    
    private static TimeSpan? ParseMonthStringToTimeSpan(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return null;
        }

        input = input.Trim().ToLowerInvariant();

        var monthParts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (monthParts.Length > 0
            && int.TryParse(monthParts[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out var months))
        {
            return TimeSpan.FromDays(months * 30.44);
        }

        return null;
    }
    
    private static WageUnit? MapWageUnit(string? unit)
    {
        return unit?.ToLower() switch
        {
            "daily" => WageUnit.Day,
            "weekly" => WageUnit.Week,
            "monthly" => WageUnit.Month,
            "annually" or "yearly" => WageUnit.Year,
            _ => null
        };
    }

    private static decimal? ParseWageAmount(string? wageInfo)
    {
        if (string.IsNullOrWhiteSpace(wageInfo))
        {
            return null;
        }

        var digitsOnly = new string(wageInfo.Where(c => char.IsDigit(c) || c == '.' || c == ',').ToArray());

        if (string.IsNullOrEmpty(digitsOnly))
        {
            return null;
        }

        digitsOnly = digitsOnly.Replace(",", "");

        return decimal.TryParse(digitsOnly, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var result)
            ? result
            : null;
    }

    private static ApprenticeshipLevel? MapApprenticeshipLevel(string? apprenticeshipLevel)
    {
        if (!Enum.TryParse<ApprenticeshipLevel>(apprenticeshipLevel, out var level))
        {
            return null;
        };

        return level;
    }
}