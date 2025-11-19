using feat.common.Models.Enums;
using feat.web.Models;
using feat.web.Models.ViewModels;

namespace feat.web.Extensions;

public static class DtoMapper
{
    private static ClientFacet ToClientFacet(this Facet facet)
    {
        return new ClientFacet
        {
            Name = facet.Name,
            Values = new Dictionary<string, long>(facet.Values) 
        };
    }
    
    public static List<ClientFacet> ToClientFacets(this List<Facet> facets)
    {
        return facets.Count > 0 ? facets.Select(f => f.ToClientFacet()).ToList() : [];
    }
    
    private static T MapCourseDetailsBase<T>(CourseDetailsResponse response) where T : CourseDetailsBase, new()
    {
        return new T
        {
            Title = response.Title,
            EntryType = response.EntryType,
            CourseType = response.CourseType ?? CourseType.Unknown,
            Level = (int?)response.Level,
            EntryRequirements = response.EntryRequirements,
            Description = response.Description,
            DeliveryMode = (DeliveryMode?)response.DeliveryMode,
            Duration = response.Duration,
            Hours = response.HoursType,
            CourseUrl = response.CourseUrl
        };
    }
    
    public static CourseDetailsCourse ToCourseViewModel(this CourseDetailsResponse response)
    {
        var model = MapCourseDetailsBase<CourseDetailsCourse>(response);

        model.Cost = response.Costs.FirstOrDefault();
        model.ProviderName = response.ProviderName;
        model.ProviderAddresses = response.ProviderAddresses.ToLocationViewModels();
        model.ProviderUrl = response.ProviderUrl;
        model.StartDates = response.StartDates
            .Where(d => d.HasValue)
            .Select(d => new StartDate(d!.Value))
            .ToList();

        return model;
    }
    
    public static CourseDetailsApprenticeship ToApprenticeshipViewModel(this CourseDetailsResponse response)
    {
        var model = MapCourseDetailsBase<CourseDetailsApprenticeship>(response);

        model.Wage = response.Wage;
        model.PositionAvailable = response.PositionAvailable;
        model.EmployerName = response.EmployerName;
        model.EmployerAddress = response.EmployerAddresses.ToLocationViewModels().FirstOrDefault();
        model.EmployerUrl = response.EmployerUrl;
        model.EmployerDescription = response.EmployerDescription;
        model.TrainingProvider = response.TrainingProvider;
        model.StartDate = response.StartDates
            .Where(d => d.HasValue)
            .Select(d => new StartDate(d!.Value))
            .FirstOrDefault();

        return model;
    }
    
    public static CourseDetailsUniversity ToUniversityViewModel(this CourseDetailsResponse response)
    {
        var model = MapCourseDetailsBase<CourseDetailsUniversity>(response);

        model.TuitionFee = response.TuitionFee;
        model.AwardingOrganisation = response.AwardingOrganisation;
        model.University = response.University;
        model.CampusName = response.CampusName;
        model.CampusAddress = response.ProviderAddresses.ToLocationViewModels().FirstOrDefault();
        model.UniversityUrl = response.ProviderUrl;
        model.StartDate = response.StartDates
            .Where(d => d.HasValue)
            .Select(d => new StartDate(d!.Value))
            .FirstOrDefault();

        return model;
    }

    private static List<Models.ViewModels.Location> ToLocationViewModels(this IEnumerable<Models.Location> locations)
    {
        return locations.Select(l => new Models.ViewModels.Location
        {
            Address1 = l.Address1,
            Address2 = l.Address2,
            Address3 = l.Address3,
            Address4 = l.Address4,
            Town = l.Town,
            County = l.County,
            Postcode = l.Postcode,
            GeoLocation = l.GeoLocation
        }).ToList();
    }
}