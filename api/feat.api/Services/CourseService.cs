using feat.api.Models;
using feat.api.Data;
using feat.api.Extensions;
using feat.common.Models.Enums;
using Microsoft.EntityFrameworkCore;
using ZiggyCreatures.Caching.Fusion;
using Location = feat.api.Models.Location;

namespace feat.api.Services;

public class CourseService(CourseDbContext dbContext, IFusionCache cache) : ICourseService
{
    public async Task<CourseDetailsResponse?> GetCourseByInstanceIdAsync(Guid instanceId)
    {
        return await cache.GetOrSetAsync<CourseDetailsResponse?>(
            $"{instanceId}",
            async entry => await GetCourseFromDatabase(instanceId));
    }
    
    private async Task<CourseDetailsResponse?> GetCourseFromDatabase(Guid instanceId)
    {
        var entryInstance = await dbContext.EntryInstances
            .Include(instance => instance.Entry.EntryCosts)
            .Include(instance => instance.Entry.Provider)
            .ThenInclude(provider => provider.ProviderLocations)
            .ThenInclude(providerLocation => providerLocation.Location)
            .Include(instance => instance.Location)
            .Include(instance => instance.Entry.Vacancies)
            .ThenInclude(vacancy => vacancy.Employer)
            .ThenInclude(employer => employer.EmployerLocations)
            .ThenInclude(employerLocation => employerLocation.Location)
            .Include(instance => instance.Entry.UniversityCourses)
            .Include(instance => instance.Entry.EntryInstances).ThenInclude(entryInstance => entryInstance.Location)
            .AsSplitQuery()
            .FirstOrDefaultAsync(e => e.Id == instanceId);

        if (entryInstance == null)
        {
            return null;
        }

        var courseDetails = new CourseDetailsResponse
        {
            Id = entryInstance.Id,
            Title = entryInstance.Entry.Title,
            EntryType = entryInstance.Entry.Type,
            CourseType = entryInstance.Entry.CourseType,
            Level = entryInstance.Entry.Level,
            EntryRequirements = entryInstance.Entry.EntryRequirements,
            Description = entryInstance.Entry.Description,
            DeliveryMode = entryInstance.StudyMode,
            Duration = entryInstance.Duration,
            HoursType = entryInstance.Entry.AttendancePattern,
            StartDates = entryInstance.Entry.EntryInstances.Select(e => e.StartDate).Distinct(),
            WhatYouWillLearn = entryInstance.Entry.WhatYouWillLearn,
            ProviderName = entryInstance.Entry.Provider.Name,
            ProviderAddresses = entryInstance.Entry.Provider.ProviderLocations
                .Select(el => el.Location.ToLocation()).Distinct().ToList(),
            ProviderUrl = entryInstance.Entry.Provider.ProviderLocations
                .Select(pl => pl.Location)
                .Select(l => l.Url)
                .FirstOrDefault(u => !string.IsNullOrEmpty(u)),
            CourseUrl = entryInstance.Entry.Url,
            AlternativeCourses = entryInstance.Entry.EntryInstances.Where(ei => ei.Id != entryInstance.Id)
                .Select(ei => ei.Id)
        };
        
        // Get our course addresses - always have the first address set to the one from the current instance
        var addresses = new List<Location>();

        // Add our instance location if we have one
        if (entryInstance.Location != null)
        {
            addresses.Add(entryInstance.Location.ToLocation());
        }

        // Add other course instance locations if we have them
        addresses.AddRange(
            entryInstance.Entry.EntryInstances
                .Where(ei => ei.Id != entryInstance.Id && ei.Location != null)
                .Select(ei => ei.Location).Distinct()
                .Select(l => (l ?? throw new ArgumentNullException(nameof(l))).ToLocation())
        );
        
        // If we have no course addresses still, add the provider's first address
        if (addresses.Count == 0 && courseDetails.ProviderAddresses.Any())
        {
            addresses.Add(courseDetails.ProviderAddresses.First());
        }

        courseDetails.CourseAddresses = addresses.Distinct();

        switch (entryInstance.Entry.Type)
        {
            case EntryType.Apprenticeship:
                var vacancy = entryInstance.Entry.Vacancies.First();
                var employer = vacancy.Employer;
                courseDetails.Wage = vacancy.Wage;
                courseDetails.TrainingProvider = entryInstance.Entry.Provider.Name;
                courseDetails.EmployerUrl = employer.Url;
                courseDetails.EmployerName = employer.Name;
                courseDetails.EmployerAddresses = employer.EmployerLocations
                    .Select(el => el.Location.ToLocation()).Distinct().ToList();
                courseDetails.EmployerDescription = null; // We don't have this
                courseDetails.PositionAvailable = null; // We don't have this
                break;
            
            case EntryType.Course:
                courseDetails.Costs = entryInstance.Entry.EntryCosts.Select(ec => (decimal?)ec.Value);
                break;
            
            case EntryType.UniversityCourse:
                courseDetails.Costs = entryInstance.Entry.EntryCosts.Select(ec => (decimal?)ec.Value);
                courseDetails.TuitionFee = courseDetails.Costs.FirstOrDefault();
                courseDetails.University = courseDetails.ProviderName;
                courseDetails.CampusName = entryInstance.Location?.Name;
                courseDetails.AwardingOrganisation = null;
                break;
        }

        return courseDetails;
    }
}