using System.Text.RegularExpressions;
using feat.common.Extensions;
using feat.common.Models.Enums;
using feat.web.Models;
using feat.web.Models.ViewModels;

namespace feat.web.Extensions;

public static class DtoMapper
{
    public static List<Models.ViewModels.Facet> ToViewModels(this IEnumerable<Models.Facet> facets)
    {
        return facets.Select(ToViewModel).ToList();
    }
    
    public static SearchResult ToResultViewModel(this Course course)
    {
        return new SearchResult
        {
            Id = course.Id,
            InstanceId = course.InstanceId,
            CourseTitle = course.Title,
            ProviderName = course.Provider,
            Location = course.Location,
            LocationName = course.LocationName,
            Distance = course.Distance,
            CourseType = course.CourseType,
            DeliveryMode = course.DeliveryMode.ToDeliveryMode(),
            IsNational =  course.IsNational,
            Requirements = course.Requirements,
            Overview = course.Overview
        };
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
            WhatYouWillLearn = response.WhatYouWillLearn,
            DeliveryMode = response.DeliveryMode.ToDeliveryMode(),
            Duration = response.Duration,
            Hours = response.HoursType,
            CourseUrl = response.CourseUrl
        };
    }
    
    extension(CourseDetailsResponse response)
    {
        public CourseDetailsCourse ToCourseViewModel()
        {
            var model = MapCourseDetailsBase<CourseDetailsCourse>(response);

            model.Cost = response.Costs.FirstOrDefault();
            model.ProviderName = response.ProviderName;
            model.CourseAddresses = response.CourseAddresses.ToLocationViewModels();
            model.ProviderUrl = response.ProviderUrl;
            model.StartDates = response.StartDates
                .Where(d => d.HasValue)
                .Select(d => new StartDate(d!.Value))
                .ToList();

            return model;
        }

        public CourseDetailsApprenticeship ToApprenticeshipViewModel()
        {
            var model = MapCourseDetailsBase<CourseDetailsApprenticeship>(response);

            model.Wage = response.Wage;
            model.PositionsAvailable = response.PositionsAvailable;
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

        public CourseDetailsUniversity ToUniversityViewModel()
        {
            var model = MapCourseDetailsBase<CourseDetailsUniversity>(response);

            model.TuitionFee = response.TuitionFee;
            model.AwardingOrganisation = response.AwardingOrganisation;
            model.University = response.University;
            model.CampusName = response.CampusName;
            model.CampusAddresses = response.CourseAddresses.ToLocationViewModels();
            model.UniversityUrl = response.ProviderUrl;
            model.StartDates = response.StartDates
                .Where(d => d.HasValue)
                .Select(d => new StartDate(d!.Value))
                .ToList();

            return model;
        }
    }

    private static List<Models.ViewModels.Location> ToLocationViewModels(this IEnumerable<Models.Location> locations)
    {
        return locations.Select(l => new Models.ViewModels.Location
        {
            Name = l.Name,
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
    
    private static Models.ViewModels.Facet ToViewModel(this Models.Facet facet)
    {
        var enumsAssembly = typeof(LearningMethod).Assembly;
        
        var enumType = enumsAssembly
            .GetTypes()
            .FirstOrDefault(t => t.IsEnum && t.Name == facet.Name);
        
        var facetDisplayName = Regex.Replace(enumType!.Name, "(\\B[A-Z])", " $1");
        
        var mappedFacet = new Models.ViewModels.Facet
        {
            Name = enumType.Name,
            DisplayName = char.ToUpper(facetDisplayName[0]) + facetDisplayName[1..].ToLower(),
            Index = GetFacetIndex(enumType.Name)
        };
        
        foreach (var enumValue in Enum.GetValues(enumType).Cast<Enum>())
        {
            mappedFacet.Values.Add(new FacetValue
            {
                Name =  enumValue.ToString(),
                DisplayName = enumValue.GetDescription(),
                Available = facet.Values.ContainsKey(enumValue.ToString()),
                Index = enumValue.GetOrder()
            });
        }

        return mappedFacet;
    }

    private static int GetFacetIndex(string enumName)
    {
        return enumName switch
        {
            nameof(CourseType) => 0,
            nameof(QualificationLevel) => 1,
            nameof(LearningMethod) => 2,
            nameof(CourseHours) => 3,
            nameof(StudyTime) => 4,
            _ => 5
        };
    }
    
    public static DeliveryMode? ToDeliveryMode(this LearningMethod? learningMethod)
    {
        return learningMethod switch
        {
            LearningMethod.Online => DeliveryMode.Online,
            LearningMethod.ClassroomBased => DeliveryMode.ClassroomBased,
            LearningMethod.Hybrid => DeliveryMode.BlendedLearning,
            LearningMethod.Workbased => DeliveryMode.WorkBased,
            _ => null
        };
    }
}