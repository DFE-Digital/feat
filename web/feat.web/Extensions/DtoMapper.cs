using feat.web.Models;
using feat.web.Models.ViewModels;

namespace feat.web.Extensions;

public static class DtoMapper
{
    // Map facets to a ClientFacet 
    public static ClientFacet ToClientFacet(this Facet facet)
    {
        return new ClientFacet() 
        {
            Name = facet.Name,
            Values = new Dictionary<string, long>(facet.Values) 
        };
    }
    
    public static List<ClientFacet> ToClientFacets(this List<Facet> facets)
    {
        return facets?.Count > 0 ? facets.Select(f => f.ToClientFacet()).ToList() : [];
    }

    public static Course ToCourse(this SearchResult searchResult)
    {
        return new Course()
        {
            CourseId = searchResult.CourseId,
            DistanceSudo = searchResult.DistanceSudo,
            CourseTitle = searchResult.CourseTitle,
            ProviderName = searchResult.ProviderName,
            Location = searchResult.Location,
            Overview = searchResult.Overview,
            CourseType = searchResult.CourseType,
            Distance = searchResult.Distance,
            Requirements = searchResult.Requirements
        };
    }

    public static List<Course> ToCourses(this List<SearchResult> searchResults)
    {
        return searchResults?.Count > 0 ? searchResults.Select(sr => sr.ToCourse()).ToList() : [];
    }
    
    // --------------------------------------------------------
    // Map server entity with a matching client entity
    // Server                               --> Client
    // ---------------------------------------------------------
    // CourseDetailsUniversity              --> UniversityCourse
    // CourseDetailsApprenticeship          --> ApprenticeshipCourse
    // CourseDetailsMultiLocationStartDate  --> MultiLocationStartDateCourse
    // CourseDetailsCourse                  --> GenericCourseDetails
    
    public static UniversityCourse ToUniversityCourse(this CourseDetailsUniversity courseDetailsUniversity)
    {
        return new UniversityCourse()
        {
            University = courseDetailsUniversity.University
        };
    }
    
}