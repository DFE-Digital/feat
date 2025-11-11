using feat.api.Models;
using feat.api.Data;
using feat.common.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Location = feat.api.Models.Location;

namespace feat.api.Services;

public class CourseService(IngestionDbContext dbContext) : ICourseService
{
    public async Task<CourseDetailsResponse?> GetCourseByIdAsync(Guid courseId)
    {
        var entry = await dbContext.Entries
            .AsNoTracking()
            .Include(e => e.Provider)
            .ThenInclude(provider => provider.ProviderLocations)
            .ThenInclude(providerLocation => providerLocation.Location)
            .Include(e => e.EntryCosts)
            .Include(e => e.EntryInstances)
            .Include(e => e.Vacancies).ThenInclude(vacancy => vacancy.Employer)
            .ThenInclude(employer => employer.EmployerLocations)
            .ThenInclude(employerLocation => employerLocation.Location)
            .FirstOrDefaultAsync(e => e.Id == courseId);

        if (entry == null)
        {
            return null;
        }

        var courseDetails = new CourseDetailsResponse
        {
            Id = entry.Id,
            Title = entry.Title,
            Type = (int?)entry.Type,
            Level = entry.Level,
            EntryRequirements = entry.EntryRequirements,
            Description = entry.Description,
            DeliveryMode = (int?)entry.EntryInstances.First().StudyMode,
            Duration = entry.EntryInstances.First().Duration,
            HoursType = (int?)entry.AttendancePattern,
            StartDates = entry.EntryInstances.Select(e => e.StartDate),
        };

        switch (entry.Type)
        {
            case EntryType.Apprenticeship:
                var vacancy = entry.Vacancies.First();
                var employer = vacancy.Employer;
                courseDetails.Wage = vacancy.Wage;
                courseDetails.TrainingProvider = entry.Provider.Name;
                courseDetails.ProviderUrl = employer.Url;
                courseDetails.EmployerName = employer.Name;
                courseDetails.EmployerAddresses = employer.EmployerLocations
                    .Select(el => el.Location)
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
                    }).ToList();
                courseDetails.EmployerDescription = null; // We don't have this
                courseDetails.CourseUrl = employer.Url; // Only URL we have that isn't the old gov site one
                courseDetails.PositionAvailable = entry.Title; // We don't have any other info, besides using entry.AimOrAltTitle
                break;
            case EntryType.Course:
                courseDetails.Costs = null;
                courseDetails.CourseUrl = null;
                courseDetails.ProviderUrl = null;
                courseDetails.ProviderName = null;
                courseDetails.ProviderAddresses = null;
                break;
            case EntryType.UniversityCourse:
                courseDetails.Costs = null;
                courseDetails.CourseUrl = null;
                courseDetails.ProviderUrl = null;
                courseDetails.ProviderName = null;
                courseDetails.ProviderAddresses = null;
                courseDetails.TuitionFee = null;
                courseDetails.University = null;
                courseDetails.CampusName = null;
                courseDetails.AwardingOrganisation = null;
                break;
        }

        return courseDetails;
    }
}