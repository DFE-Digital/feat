using feat.api.Models;
using feat.api.Data;
using feat.common.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Location = feat.api.Models.Location;

namespace feat.api.Services;

public class CourseService(CourseDbContext dbContext) : ICourseService
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
            .ThenInclude(employerLocation => employerLocation.Location).Include(entry => entry.UniversityCourses)
            .FirstOrDefaultAsync(e => e.Id == courseId);

        if (entry == null)
        {
            return null;
        }

        var courseDetails = new CourseDetailsResponse
        {
            Id = entry.Id,
            Title = entry.Title,
            EntryType = entry.Type,
            CourseType = entry.CourseType,
            Level = entry.Level,
            EntryRequirements = entry.EntryRequirements,
            Description = entry.Description,
            DeliveryMode = entry.EntryInstances.First().StudyMode,
            Duration = entry.EntryInstances.First().Duration,
            HoursType = entry.AttendancePattern,
            StartDates = entry.EntryInstances.Select(e => e.StartDate)
        };

        switch (entry.Type)
        {
            case EntryType.Apprenticeship:
                var vacancy = entry.Vacancies.First();
                var employer = vacancy.Employer;
                courseDetails.Wage = vacancy.Wage;
                courseDetails.TrainingProvider = entry.Provider.Name;
                courseDetails.EmployerUrl = employer.Url;
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
                courseDetails.CourseUrl = !string.IsNullOrEmpty(entry.Url) ? entry.Url : vacancy.Url;
                courseDetails.EmployerDescription = null; // We don't have this
                courseDetails.PositionAvailable = entry.AimOrAltTitle; // We don't have this
                break;
            case EntryType.Course:
                courseDetails.Costs = entry.EntryCosts.Select(ec => (decimal?)ec.Value);
                courseDetails.CourseUrl = entry.Url;
                courseDetails.ProviderName = entry.Provider.Name;
                courseDetails.ProviderAddresses = entry.Provider.ProviderLocations
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
                // TODO: Set these
                courseDetails.ProviderUrl = null; // Need to get this from Location.Url
                break;
            case EntryType.UniversityCourse:
                courseDetails.Costs = entry.EntryCosts.Select(ec => (decimal?)ec.Value);
                courseDetails.CourseUrl = entry.Url;
                courseDetails.ProviderName = entry.Provider.Name;
                courseDetails.ProviderAddresses = entry.Provider.ProviderLocations
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
                // TODO: Set these
                courseDetails.ProviderUrl = null; // Need to get this from Location.Url
                courseDetails.TuitionFee = null; // Can just be Costs.First()
                courseDetails.University = null;
                courseDetails.CampusName = null;
                courseDetails.AwardingOrganisation = null;
                break;
        }

        return courseDetails;
    }
}