using System.Globalization;
using System.Text;
using feat.common;
using feat.common.Extensions;
using feat.common.Models;
using feat.common.Models.AiSearch;
using feat.common.Models.Enums;
using feat.ingestion.Configuration;
using feat.ingestion.Data;
using feat.ingestion.Enums;
using feat.ingestion.Models.FAA;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using Z.BulkOperations;
using External = feat.ingestion.Models.FAA.External;
using Location = feat.common.Models.Location;

namespace feat.ingestion.Handlers.FAA;

public class FaaIngestionHandler(
    IngestionOptions options,
    IApiClient apiClient,
    IngestionDbContext dbContext,
    ISearchIndexHandler searchIndexHandler)
    : IngestionHandler(options)
{
    private readonly IngestionOptions _options = options;

    public override IngestionType IngestionType => IngestionType.Api | IngestionType.Automatic;
    public override string Name => "Find An Apprenticeship";
    public override string Description => "Ingestion from Find An Apprenticeship API";
    public override SourceSystem SourceSystem => SourceSystem.FAA;

    public override async Task<bool> ValidateAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(_options.ApprenticeshipApiKey))
        {
            Console.WriteLine("Find An Apprenticeship API key not set.");
            return false;
        }

        const string url = "vacancies/vacancy?PageNumber=1&PageSize=1";
        External.ApiResponse response;

        try
        {
            response = await apiClient.GetAsync<External.ApiResponse>(
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
            var allApprenticeships = new List<Apprenticeship>();
            var allAddresses = new List<Address>();
            var pageNumber = 1;

            while (true)
            {
                Console.WriteLine($"Fetching page {pageNumber} (PageSize={apiPageSize})...");
                
                var url = $"vacancies/vacancy?PageNumber={pageNumber}&PageSize={apiPageSize}";
                
                var response = await apiClient.GetAsync<External.ApiResponse>(
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
    
    public override async Task<bool> SyncAsync(CancellationToken cancellationToken)
    {
        var resultInfo = new ResultInfo();
        var auditEntries = new List<AuditEntry>();
        bool skip = false;

        Console.WriteLine($"Starting sync of {Name} data");

        if (skip)
        {
            Console.WriteLine("Skipped");
            return true;
        }
        
        var apprenticeships = dbContext.Set<Apprenticeship>()
            .Include(apprenticeship => apprenticeship.Addresses)
            .ToList();

        if (apprenticeships.Count == 0)
        {
            Console.WriteLine("No data found in staging tables.");
            return false;
        }

        Console.WriteLine($"Processing {apprenticeships.Count} apprenticeships from staging...");

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        
        // PROVIDER
        Console.WriteLine("Generating providers...");
        var providers = apprenticeships
            .GroupBy(a => a.Ukprn)
            .Select(a => new Provider
        {
            Created = DateTime.UtcNow,
            Name = a.First().ProviderName ?? "Unknown provider",
            Ukprn = a.Key.ToString(),
            SourceSystem = SourceSystem,
            SourceReference = a.Key.ToString()
        }).ToList();
        
        await dbContext.BulkSynchronizeAsync(providers, options =>
        {
            options.IgnoreOnSynchronizeUpdateExpression = p => new
            {
                p.Id,
                p.Created
            };
            options.ColumnPrimaryKeyExpression = p => p.Ukprn;
            options.ColumnSynchronizeDeleteKeySubsetExpression = s => s.SourceSystem;
            options.UseRowsAffected = true;
            options.ResultInfo = resultInfo;
        }, cancellationToken);
        Console.WriteLine($"{resultInfo.RowsAffectedInserted} created");
        Console.WriteLine($"{resultInfo.RowsAffectedUpdated} updated");
        Console.WriteLine($"{resultInfo.RowsAffectedDeleted} deleted");
        resultInfo = new ResultInfo();

        var providerLookup = dbContext.Set<Provider>()
            .ToDictionary(p => p.Ukprn!, p => p.Id);

        // SECTOR
        Console.WriteLine("Generating sectors...");
 
        var sectors = apprenticeships
            .Where(a => !string.IsNullOrEmpty(a.CourseRoute))
            .GroupBy(a => a.CourseRoute!.Trim(), StringComparer.OrdinalIgnoreCase)
            .Select(a => new Sector
            {
                Name = a.Key,
                SourceSystem = SourceSystem
            }).ToList();

        await dbContext.BulkSynchronizeAsync(sectors, options =>
        {
            options.IgnoreOnSynchronizeUpdateExpression = s => new
            {
                s.Id,
            };
            options.ColumnPrimaryKeyExpression = s => s.Name;
            options.ColumnSynchronizeDeleteKeySubsetExpression = s => s.SourceSystem;options.UseRowsAffected = true;
            options.ResultInfo = resultInfo;
        }, cancellationToken);
        Console.WriteLine($"{resultInfo.RowsAffectedInserted} created");
        Console.WriteLine($"{resultInfo.RowsAffectedUpdated} updated");
        Console.WriteLine($"{resultInfo.RowsAffectedDeleted} deleted");
        resultInfo = new ResultInfo();

        var sectorLookup = dbContext.Set<Sector>()
            .ToDictionary(s => s.Name.ToLower().Trim(), s => s.Id);
        
        // ENTRY
        var entries = apprenticeships.Select(a =>
        {
            var providerId = providerLookup[a.Ukprn.ToString()];
            return new Entry
            {
                Created = DateTime.UtcNow,
                ProviderId = providerId,
                Reference = a.VacancyReference!,
                Title = a.Title!,
                AimOrAltTitle = a.CourseTitle!,
                Description = a.Description,
                FlexibleStart = a.StartDate == null,
                AttendancePattern = MapCourseHours(a.HoursPerWeek),
                Url = a.ApplicationUrl ?? string.Empty,
                Type = EntryType.Apprenticeship,
                Level = MapCourseLevel(a.CourseLevel),
                StudyTime = StudyTime.Daytime,
                SourceSystem = SourceSystem,
                SourceReference = a.VacancyReference!
            };
        }).ToList();
        Console.WriteLine($"Generating entries for {entries.LongCount()} courses...");
        
        await dbContext.BulkSynchronizeAsync(entries, options =>
        {
            options.IgnoreOnSynchronizeUpdateExpression = p => new
            {
                p.Id,
                p.Created,
                p.Updated,
                p.SourceUpdated,
                p.IngestionState
            };
            options.UseRowsAffected = true;
            options.ColumnPrimaryKeyExpression = e => e.SourceReference;
            options.ColumnSynchronizeDeleteKeySubsetExpression = e => e.SourceSystem;
            options.UseAudit = true;
            options.AuditEntries = auditEntries;
            options.ResultInfo = resultInfo;
        }, cancellationToken);
        Console.WriteLine($"{resultInfo.RowsAffectedInserted} created");
        Console.WriteLine($"{resultInfo.RowsAffectedUpdated} updated");
        Console.WriteLine($"{resultInfo.RowsAffectedDeleted} deleted");
        resultInfo = new ResultInfo();
        // Run through the audit entries and check to see which entries were created or updated
        var createdIds = auditEntries.Where(e => e.Action == AuditActionType.Insert)
            .SelectMany(e => e.Values.Where(ae => ae.ColumnName == "Id").Select(ae => (Guid)ae.NewValue));
        // For all of our created entries, we'll need to set those to be indexed
        var createdEntries = dbContext.Entries.WhereBulkContains(createdIds);
        await createdEntries.ForEachAsync(e => e.IngestionState = IngestionState.Pending,
            cancellationToken: cancellationToken);

        // We're only interested here if any text fields have changed
        var updatedIds = auditEntries.Where(e => e.Action == AuditActionType.Update
                                                 && e.Values.Exists(ae =>
                                                     ae.ColumnName is "Title" or "AimOrAltTitle" or "Description" &&
                                                     !Equals(ae.OldValue, ae.NewValue)))
            .SelectMany(e => e.Values.Where(ae => ae.ColumnName == "Id").Select(ae => (Guid)ae.NewValue));

        var updatedEntries = dbContext.Entries.WhereBulkContains(updatedIds);
        await updatedEntries.ForEachAsync(e => e.IngestionState = IngestionState.Pending,
            cancellationToken: cancellationToken);
        await dbContext.BulkSaveChangesAsync(cancellationToken);
        
        
        var entryLookup = dbContext.Set<Entry>()
            .Where(e => e.SourceSystem == SourceSystem)
            .ToDictionary(e => e.SourceReference, e => e.Id);
        
        // ENTRYSECTOR
        Console.WriteLine("Generating entry sectors...");

        var entrySectors = apprenticeships
            .Where(a => !string.IsNullOrEmpty(a.CourseRoute))
            .Select(a => new EntrySector
            {
                EntryId = entryLookup[a.VacancyReference!],
                SectorId = sectorLookup[a.CourseRoute!.ToLower().Trim()],
            SourceSystem = SourceSystem
        }).ToList();

        await dbContext.BulkSynchronizeAsync(entrySectors, options =>
        {
            options.ColumnPrimaryKeyExpression = es => new
            {
                es.EntryId,
                es.SectorId
            };
            options.ColumnSynchronizeDeleteKeySubsetExpression = l => l.SourceSystem;
            options.UseRowsAffected = true;
            options.ResultInfo = resultInfo;
        }, cancellationToken);

        Console.WriteLine($"{resultInfo.RowsAffectedInserted} created");
        Console.WriteLine($"{resultInfo.RowsAffectedUpdated} updated");
        Console.WriteLine($"{resultInfo.RowsAffectedDeleted} deleted");
        resultInfo = new ResultInfo();
        
        // EMPLOYER
        Console.WriteLine("Generating employers...");
        
        var employers = apprenticeships
            .GroupBy(a => a.EmployerName!.Trim(), StringComparer.OrdinalIgnoreCase)
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
            options.UseRowsAffected = true;
            options.ResultInfo = resultInfo;
        }, cancellationToken);

        Console.WriteLine($"{resultInfo.RowsAffectedInserted} created");
        Console.WriteLine($"{resultInfo.RowsAffectedUpdated} updated");
        Console.WriteLine($"{resultInfo.RowsAffectedDeleted} deleted");
        resultInfo = new ResultInfo();

        var employerLookup = dbContext.Set<Employer>()
            .ToDictionary(e => e.Name.ToLower().Trim(), e => e.Id);

        // VACANCY
        Console.WriteLine("Generating vacancies...");
        
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
            options.UseRowsAffected = true;
            options.ResultInfo = resultInfo;
        }, cancellationToken);

        Console.WriteLine($"{resultInfo.RowsAffectedInserted} created");
        Console.WriteLine($"{resultInfo.RowsAffectedUpdated} updated");
        Console.WriteLine($"{resultInfo.RowsAffectedDeleted} deleted");
        resultInfo = new ResultInfo();
        
        // LOCATION
        Console.WriteLine("Generating locations...");
        
        var locations = apprenticeships
            .SelectMany(a => a.Addresses)
            .GroupBy(a => new
            {
                Postcode = a.Postcode?.Trim(),
                AddressLine1 = a.AddressLine1?.Trim(),
                AddressLine4 = a.AddressLine4?.Trim()
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
                    Address4 = a.Key.AddressLine4,
                    Postcode = a.Key.Postcode,
                    GeoLocation = longitude != null && latitude != null
                        ? new Point(longitude.Value, latitude.Value) { SRID = 4326 }
                        : null,
                    SourceSystem = SourceSystem,
                    SourceReference = $"{a.First().Id}"
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
            options.AllowDuplicateKeys = true;
            options.ColumnSynchronizeDeleteKeySubsetExpression = e => e.SourceSystem;
            options.UseRowsAffected = true;
            options.ResultInfo = resultInfo;
        }, cancellationToken);

        Console.WriteLine($"{resultInfo.RowsAffectedInserted} created");
        Console.WriteLine($"{resultInfo.RowsAffectedUpdated} updated");
        Console.WriteLine($"{resultInfo.RowsAffectedDeleted} deleted");
        resultInfo = new ResultInfo();
        
        var locationLookup = new Dictionary<(string?, string?, string?), Guid>();

        foreach (var location in dbContext.Set<Location>())
        {
            var key = (
                location.Postcode?.ToLower().Trim(),
                location.Address1?.ToLower().Trim(),
                location.Address4?.ToLower().Trim()
            );

            if (!locationLookup.ContainsKey(key))
            {
                locationLookup[key] = location.Id;
            }
        }
        
        // ENTRYINSTANCE
        Console.WriteLine($"Generating entry instances ...");
        var entryInstances = new List<EntryInstance>();

        foreach (var apprenticeship in apprenticeships)
        {
            var entryId = entryLookup[apprenticeship.VacancyReference!];
            
            if (apprenticeship.Addresses.Count > 0)
            {
                // Loop through and create an instance per address
                foreach (var address in apprenticeship.Addresses)
                {
                    var locationId = locationLookup[(
                        address.Postcode?.ToLower().Trim(),
                        address.AddressLine1?.ToLower().Trim(),
                        address.AddressLine4?.ToLower().Trim())];
                    
                    entryInstances.Add(new EntryInstance
                    {
                        Created = DateTime.UtcNow,
                        EntryId = entryId,
                        StartDate = apprenticeship.StartDate,
                        Duration = ParseMonthStringToTimeSpan(apprenticeship.ExpectedDuration, apprenticeship.StartDate),
                        StudyMode = LearningMethod.Workbased,
                        Reference = $"{apprenticeship.VacancyReference}_{address.Id}",
                        LocationId = locationId,
                        SourceSystem = SourceSystem,
                        SourceReference = $"{apprenticeship.VacancyReference}_{address.Id}"
                    });
                }
            }
            else
            {
                // Assume this is a national vacancy
                entryInstances.Add(new EntryInstance
                {
                    Created = DateTime.UtcNow,
                    EntryId = entryId,
                    StartDate = apprenticeship.StartDate,
                    Duration = ParseMonthStringToTimeSpan(apprenticeship.ExpectedDuration, apprenticeship.StartDate),
                    StudyMode = LearningMethod.Workbased,
                    Reference = $"{apprenticeship.VacancyReference}",
                    SourceSystem = SourceSystem,
                    SourceReference = $"{apprenticeship.VacancyReference}"
                });
            }
        }
        
        await dbContext.BulkSynchronizeAsync(entryInstances, options =>
        {
            options.IgnoreOnSynchronizeUpdateExpression = ei => new
            {
                ei.Id,
                ei.Created
            };
            options.ColumnPrimaryKeyExpression = ei => ei.SourceReference;
            options.ColumnSynchronizeDeleteKeySubsetExpression = ei => ei.SourceSystem;
            options.UseRowsAffected = true;
            options.ResultInfo = resultInfo;
        }, cancellationToken);
        Console.WriteLine($"{resultInfo.RowsAffectedInserted} created");
        Console.WriteLine($"{resultInfo.RowsAffectedUpdated} updated");
        Console.WriteLine($"{resultInfo.RowsAffectedDeleted} deleted");
        resultInfo = new ResultInfo();
        
        // EMPLOYERLOCATION
        Console.WriteLine("Generating employer locations...");
        
        var employerLocationKeys = apprenticeships
            .SelectMany(ap => ap.Addresses.Select(ad => new
            {
                EmployerName = ap.EmployerName!.ToLower().Trim(),
                Postcode = ad.Postcode?.ToLower().Trim(),
                Address1 = ad.AddressLine1?.ToLower().Trim(),
                Address4 = ad.AddressLine4?.ToLower().Trim()
            }))
            .Distinct()
            .ToList();
        
        var employerLocations = employerLocationKeys
            .Select(x => new EmployerLocation
            {
                EmployerId = employerLookup[x.EmployerName],
                LocationId = locationLookup[(x.Postcode, x.Address1, x.Address4)],
            })
            .ToList();
        
        await dbContext.BulkSynchronizeAsync(employerLocations, options =>
        {
            options.ColumnPrimaryKeyExpression = el => new
            {
                el.EmployerId,
                el.LocationId
            };           
            options.UseRowsAffected = true;
            options.ResultInfo = resultInfo;
        }, cancellationToken);

        Console.WriteLine($"{resultInfo.RowsAffectedInserted} created");
        Console.WriteLine($"{resultInfo.RowsAffectedUpdated} updated");
        Console.WriteLine($"{resultInfo.RowsAffectedDeleted} deleted");

        
        await transaction.CommitAsync(cancellationToken);
        Console.WriteLine($"{Name} table sync complete.");

        return true;
    }

    public override async Task<bool> IndexAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine($"Starting {Name} AI Search indexing...");
        var sb = new StringBuilder();
        
        while (true)
        {
            var entries = dbContext.Entries
                .Include(entry => entry.EntrySectors)
                .ThenInclude(entrySector => entrySector.Sector)
                .Include(entry => entry.EntryInstances)
                .ThenInclude(instance => instance.Location)
                .Include(entry => entry.Provider)
                .ThenInclude(provider => provider.ProviderLocations)
                .ThenInclude(providerLocation => providerLocation.Location)
                .Where(x =>
                    x.SourceSystem == SourceSystem &&
                    x.IngestionState == IngestionState.Pending)
                .Take(250)
                .ToList();

            if (entries.Count == 0)
            {
                Console.WriteLine("No entries found to index.");
                return false;
            }

            Console.WriteLine($"Loaded {entries.Count} entries for indexing.");
            
            var searchEntries = new List<AiSearchEntry>();
            foreach (var entry in entries)
            {
                // TODO: Split these into their own fields
                sb.Clear();
                sb.AppendLine(entry.Description);
                sb.AppendLine(entry.WhatYouWillLearn);
                var description = sb.ToString().Scrub();
                
                foreach (var instance in entry.EntryInstances)
                {
                    var location = instance.Location ?? entry.Provider.ProviderLocations.FirstOrDefault()?.Location;
                    var searchEntry = new AiSearchEntry
                    {
                        Id = entry.Id.ToString(),
                        InstanceId = instance.Id.ToString(),
                        Sector = string.Join(", ", entry.EntrySectors.Select(es => es.Sector.Name)),
                        Title = entry.Title,
                        LearningAimTitle = entry.AimOrAltTitle,
                        Description = description,
                        EntryType = nameof(EntryType.Apprenticeship),
                        Source = SourceSystem.ToString(),
                        QualificationLevel = entry.Level?.ToString() ?? string.Empty,
                        LearningMethod = instance.StudyMode.ToString() ?? string.Empty,
                        CourseHours = entry.AttendancePattern?.ToString() ?? string.Empty,
                        StudyTime = entry.StudyTime?.ToString() ?? string.Empty,
                        Location = location?.GeoLocation.ToGeographyPoint()
                    };

                    searchEntry.TitleVector = searchIndexHandler.GetVector(searchEntry.Title);
                    searchEntry.DescriptionVector = searchIndexHandler.GetVector(searchEntry.Description);
                    searchEntry.LearningAimTitleVector = searchIndexHandler.GetVector(searchEntry.LearningAimTitle);
                    searchEntry.SectorVector = searchIndexHandler.GetVector(searchEntry.Sector);
                    searchEntries.Add(searchEntry);
                }
                
                entry.IngestionState = IngestionState.Processing;
            }
            await dbContext.BulkSaveChangesAsync(cancellationToken);

            var resultInfo = new ResultInfo();
            await dbContext.BulkMergeAsync(searchEntries, options =>
            {
                options.ColumnPrimaryKeyExpression = ai => ai.InstanceId;
                options.UseRowsAffected = true;
                options.ResultInfo = resultInfo;
            }, cancellationToken);

            Console.WriteLine($"{resultInfo.RowsAffectedInserted} created for indexing");
            Console.WriteLine($"{resultInfo.RowsAffectedUpdated} updated for indexing");
            
            var result = await searchIndexHandler.Ingest(searchEntries);
            
            // Update the entries above to processing
            foreach (var entry in entries)
            {
                entry.IngestionState = result ? IngestionState.Complete : IngestionState.Failed;
            }
            await dbContext.BulkSaveChangesAsync(cancellationToken);

            // Keep going until we've ingested everything
            if (!dbContext.Entries.Any(e =>
                    e.IngestionState == IngestionState.Pending
                    && e.SourceSystem == SourceSystem))
            {
                Console.WriteLine($"{Name} AI Search indexing {(result ? "complete" : "failed")}.");
                return result;
            }

            // Clear our change tracking as we're going to get another batch next and
            // we don't care about the old ones
            dbContext.ChangeTracker.Clear();
        }
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
    
    private static TimeSpan? ParseMonthStringToTimeSpan(string? input, DateTime? startDate)
    {
        if (string.IsNullOrWhiteSpace(input) || startDate == null)
        {
            return null;
        }

        input = input.Trim().ToLowerInvariant();

        var monthParts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (monthParts.Length > 0
            && int.TryParse(monthParts[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out var months))
        {
            var endDate = startDate.Value.AddMonths(months);
            return (endDate - startDate).Value.Duration();
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
        }

        return level;
    }
    
    private static QualificationLevel? MapCourseLevel(int courseLevel)
    {
        if (Enum.IsDefined(typeof(QualificationLevel), courseLevel))
        {
            return (QualificationLevel)courseLevel;
        }

        return null;
    }
}