using System.Globalization;
using feat.common.Models;
using feat.common.Models.AiSearch;
using feat.common.Models.Enums;
using feat.ingestion.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Spatial;
using NetTopologySuite.Geometries;
using Location = feat.common.Models.Location;
using Database = feat.common.Models.Staging.FAA;

namespace feat.ingestion.Handlers.FAA;

public class FaaTransformHandler(IngestionDbContext dbContext)
{
    public async Task<List<AiSearchEntry>> TransformAsync(
        List<(Database.Apprenticeship Row, bool HasChanges)> rowsToTransform,
        CancellationToken cancellationToken)
    {
        var transformedCount = 0;
        var aiSearchEntries = new List<AiSearchEntry>();

        foreach (var row in rowsToTransform.Select(sa => sa.Row)) // TODO: Change
        {
            try
            {
                // Employer
                
                var employer = await dbContext.Employers
                    .Include(e => e.EmployerLocations)
                    .ThenInclude(el => el.Location)
                    .FirstOrDefaultAsync(e => e.Name == row.EmployerName, cancellationToken);

                if (employer == null)
                {
                    employer = new Employer
                    {
                        Id = Guid.NewGuid(),
                        Created = DateTime.UtcNow,
                        Name = row.EmployerName ?? "Unknown",
                        Url = row.EmployerWebsiteUrl
                    };
                    
                    dbContext.Employers.Add(employer);
                }
                else
                {
                    if (employer.Url != row.EmployerWebsiteUrl)
                    {
                        employer.Url = row.EmployerWebsiteUrl;
                    }
                }

                // Provider

                var provider = await dbContext.Providers
                    .FirstOrDefaultAsync(p => p.Ukprn == row.Ukprn.ToString(), cancellationToken);

                if (provider == null)
                {
                    provider = new Provider
                    {
                        Id = Guid.NewGuid(),
                        Created = DateTime.UtcNow,
                        Name = row.ProviderName ?? "Unknown",
                        Ukprn = row.Ukprn.ToString()
                    };
                    
                    dbContext.Providers.Add(provider);
                }
                else if (provider.Name != row.ProviderName)
                {
                    provider.Name = row.ProviderName ?? "Unknown";
                }
                
                // Entry

                var entryReference = row.VacancyReference ?? Guid.NewGuid().ToString();

                var entry = await dbContext.Entries
                    .Include(e => e.EntryInstances)
                    .Include(e => e.EntrySectors)
                    .Include(e => e.EntryLocations)
                    .ThenInclude(el => el.Location)
                    .Include(e => e.EntryCosts)
                    .FirstOrDefaultAsync(e => e.Reference == entryReference, cancellationToken);

                if (entry == null)
                {
                    entry = new Entry
                    {
                        Id = Guid.NewGuid(),
                        Created = DateTime.UtcNow,
                        Provider = provider,
                        ProviderId = provider.Id,
                        Reference = entryReference,
                        Title = row.Title ?? "Untitled Apprenticeship",
                        Description = row.Description,
                        FlexibleStart = row.StartDate == null,
                        Url = row.VacancyUrl ?? row.ApplicationUrl ?? "",
                        SourceSystem = SourceSystem.FAA,
                        SourceUpdated = row.PostedDate,
                        Type = EntryType.Apprenticeship,
                        AttendancePattern = row.HoursPerWeek < 35 ? CourseHours.PartTime : CourseHours.FullTime,
                        Level = StudyTime.Daytime, // TODO: Check
                        EntryLocations = new List<EntryLocation>(),
                        EntryInstances = new List<EntryInstance>(),
                        EntrySectors = new List<EntrySector>(),
                        EntryCosts = new List<EntryCost>()
                    };
                    
                    dbContext.Entries.Add(entry);
                }
                else
                {
                    if (entry.Title != row.Title)
                    {
                        entry.Title = row.Title ?? "Untitled Apprenticeship";
                    }

                    if (entry.Description != row.Description)
                    {
                        entry.Description = row.Description;
                    }

                    var url = row.VacancyUrl ?? row.ApplicationUrl ?? string.Empty;
                    if (entry.Url != url)
                    {
                        entry.Url = url;
                    }

                    if (entry.SourceUpdated != row.PostedDate)
                    {
                        entry.SourceUpdated = row.PostedDate;
                    }
                }
                
                // Locations
                
                foreach (var address in row.Addresses)
                {
                    var location = employer.EmployerLocations
                        .Select(el => el.Location)
                        .FirstOrDefault(l =>
                            l.Postcode == address.Postcode &&
                            l.Address1 == address.AddressLine1);

                    if (location == null)
                    {
                        location = new Location
                        {
                            Id = Guid.NewGuid(),
                            Created = DateTime.UtcNow,
                            Address1 = address.AddressLine1,
                            Address2 = address.AddressLine2,
                            Address3 = address.AddressLine3,
                            Address4 = address.AddressLine4,
                            Postcode = address.Postcode,
                            GeoLocation = address.Latitude != null && address.Longitude != null
                                ? new Point(address.Longitude.Value, address.Latitude.Value) { SRID = 4326 }
                                : null
                        };

                        dbContext.Locations.Add(location);

                        // TODO: Check locations
                        
                        dbContext.EmployerLocations.Add(new EmployerLocation
                        {
                            Id = Guid.NewGuid(),
                            Employer = employer,
                            Location = location
                        });

                        entry.EntryLocations.Add(new EntryLocation
                        {
                            Id = Guid.NewGuid(),
                            Entry = entry,
                            Location = location
                        });
                    }
                }
                
                // EntryInstance
                
                // TODO: Multiple EntryInstances?

                var entryInstance = entry.EntryInstances
                    .FirstOrDefault(ei => ei.Reference == row.VacancyReference);

                var duration = ParseMonthStringToTimeSpan(row.ExpectedDuration);

                if (entryInstance == null)
                {
                    entryInstance = new EntryInstance
                    {
                        Id = Guid.NewGuid(),
                        Created = DateTime.UtcNow,
                        Entry = entry,
                        Reference = row.VacancyReference ?? Guid.NewGuid().ToString(),
                        StartDate = row.StartDate,
                        Duration = duration,
                        StudyMode = LearningMethod.Workbased
                    };
                    
                    entry.EntryInstances.Add(entryInstance);
                }
                else
                {
                    if (entryInstance.StartDate != row.StartDate)
                    {
                        entryInstance.StartDate = row.StartDate;
                    }

                    if (entryInstance.Duration != duration)
                    {
                        entryInstance.Duration = duration;
                    }
                }
                
                // EntrySector
                
                var sector = await dbContext.Sectors
                    .FirstOrDefaultAsync(s => s.Name == row.CourseRoute, cancellationToken);

                if (sector == null)
                {
                    sector = new Sector
                    {
                        Id = Guid.NewGuid(),
                        Created = DateTime.UtcNow,
                        Name = row?.CourseRoute ?? "Unknown" // TODO: Check
                    };
                    
                    dbContext.Sectors.Add(sector);
                }

                if (entry.EntrySectors.All(es => es.SectorId != sector.Id))
                {
                    entry.EntrySectors.Add(new EntrySector
                    {
                        Id = Guid.NewGuid(),
                        Entry = entry,
                        Sector = sector
                    });
                }

                // EntryCost / Wage

                var wageValue = ParseWageAmount(row.WageAdditionalInformation);
                var entryCost = entry.EntryCosts.FirstOrDefault();

                if (wageValue != null)
                {
                    if (entryCost == null)
                    {
                        entry.EntryCosts.Add(new EntryCost
                        {
                            Id = Guid.NewGuid(),
                            Value = wageValue,
                            Description = row.WageAdditionalInformation
                        });
                    }
                    else
                    {
                        if (!entryCost.Value.Equals(wageValue) ||
                            entryCost.Description != row.WageAdditionalInformation)
                        {
                            entryCost.Value = wageValue;
                            entryCost.Description = row.WageAdditionalInformation;
                        }
                    }
                }

                // Vacancy

                var vacancy = await dbContext.Vacancies
                    .FirstOrDefaultAsync(v => v.EntryId == entry.Id, cancellationToken);

                if (vacancy == null)
                {
                    vacancy = new Vacancy
                    {
                        Id = Guid.NewGuid(),
                        Entry = entry,
                        Employer = employer,
                        Positions = (short?)row.NumberOfPositions,
                        Wage = (decimal?)wageValue,
                        WageUnit = MapWageUnit(row.WageUnit),
                        HoursPerWeek = (int?)row.HoursPerWeek,
                        ClosingDate = row.ClosingDate
                    };
                    
                    dbContext.Vacancies.Add(vacancy);
                }
                else
                {
                    if (vacancy.Positions != row.NumberOfPositions)
                    {
                        vacancy.Positions = (short?)row.NumberOfPositions;
                    }

                    if (vacancy.Wage != (decimal?)wageValue)
                    {
                        vacancy.Wage = (decimal?)wageValue;
                    }

                    if (vacancy.WageUnit != MapWageUnit(row.WageUnit))
                    {
                        vacancy.WageUnit = MapWageUnit(row.WageUnit);
                    }

                    if (vacancy.HoursPerWeek != row.HoursPerWeek)
                    {
                        vacancy.HoursPerWeek = (int?)row.HoursPerWeek;
                    }

                    if (vacancy.ClosingDate != row.ClosingDate)
                    {
                        vacancy.ClosingDate = row.ClosingDate;
                    }
                }
                
                // AISearchEntry
                
                var aiSearchEntry = new AiSearchEntry
                {
                    Id = entry.Id.ToString(),
                    InstanceId = entryInstance.Id.ToString(),
                    Title = entry.Title,
                    Description = entry.Description,
                    EntryType = nameof(EntryType.Apprenticeship),
                    Source = nameof(SourceSystem.FAA),
                    // TODO: Check all below
                    LearningAimTitle = entry.Title,
                    Sector = sector.Name,
                    Location = entry.EntryLocations.FirstOrDefault()
                                   ?.Location !=
                               null
                        ? GeographyPoint.Create(
                            entry.EntryLocations.First()
                                .Location.GeoLocation.Y,
                            entry.EntryLocations.First()
                                .Location.GeoLocation.X)
                        : null,
                    QualificationLevel = nameof(QualificationLevel.Level3),
                    LearningMethod = "TEST",
                    CourseHours = "10",
                    StudyTime = "20"
                };
                
                aiSearchEntries.Add(aiSearchEntry);

                transformedCount++;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to transform record with VacancyReference ({row.VacancyReference}): {ex.Message}");
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        Console.WriteLine($"Transformed {transformedCount} Find An Apprenticeship records into normalized schema.");

        return aiSearchEntries;
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

    private static double? ParseWageAmount(string? wageInfo)
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

        return double.TryParse(digitsOnly, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var result)
            ? result
            : null;
    }
}
