using feat.api.Models;
using feat.api.Data;
using Microsoft.EntityFrameworkCore;
using Location = feat.api.Models.Location;

namespace feat.api.Services;

public class CourseService : ICourseService
{
    private readonly IngestionDbContext _dbContext;

    public CourseService(IngestionDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CourseDetailsResponse?> GetCourseByIdAsync(Guid courseId)
    {
        var entry = await _dbContext.Entries
            .AsNoTracking()
            .Include(e => e.Provider)
            .ThenInclude(provider => provider.ProviderLocations)
            .ThenInclude(providerLocation => providerLocation.Location)
            .Include(e => e.EntryCosts)
            .Include(e => e.EntryInstances)
            .Include(e => e.Vacancies)
            .FirstOrDefaultAsync(e => e.Id == courseId);

        if (entry == null)
        {
            return null;
        }
        
        return new CourseDetailsResponse
        {
            Id = entry.Id,
            Title = entry.Title,
            Type = (int?)entry.Type,
            Level = (int?)entry.Level,
            EntryRequirements = entry.EntryRequirements,
            Description = entry.Description,
            DeliveryMode = (int?)entry.EntryInstances.FirstOrDefault()?.StudyMode, // Check
            Duration = entry.EntryInstances.FirstOrDefault()?.Duration, // Check
            Hours = (int?)entry.AttendancePattern,
            ProviderName = entry.Provider.Name,
            ProviderAddresses = entry.Provider.ProviderLocations // Check
                .Select(pl => pl.Location)
                .Select(l => new Location
                {
                    Address1 = l.Address1,
                    Address2 = l.Address2,
                    Address3 = l.Address3,
                    Address4 = l.Address4,
                    Town = l.Town,
                    County = l.County,
                    Postcode = l.Postcode,
                    GeoLocation = l.GeoLocation != null ? new GeoLocation {
                            Latitude = l.GeoLocation.Y,
                            Longitude = l.GeoLocation.X
                        } : null
                }).ToList(),
            ProviderDescription = string.Empty, // Check
            ProviderUrl = string.Empty, // Check
            StartDates = entry.EntryInstances.Select(e => e.StartDate),
            Costs = entry.EntryCosts.Select(c => c.Value), // Check
            Wage = entry.Vacancies.FirstOrDefault()?.Wage, // Check
            PositionAvailable = string.Empty, // Check
            TrainingProvider = string.Empty, // Check
            AwardingOrganisation = string.Empty, // Check
            CampusName = string.Empty, // Check
            CourseUrl = entry.Url
        };
    }
}