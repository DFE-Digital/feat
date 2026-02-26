using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using feat.api.Models;
using feat.api.Services;
using ModelContextProtocol.Server;
using ValidationResult = feat.api.Models.ValidationResult;

namespace feat.api.MCP;

[McpServerToolType]
public class McpTools(ISearchService searchService, ICourseService courseService)
{
    [McpServerTool, Description("Find courses and apprenticeships and filter accordingly. When searching by location, prefer latitude and longitude (with a radius) or polygons.")]
    public async Task<SearchResponse?> Search(
        McpSearchRequest searchRequest
        )
    {
        // If we've got everything we need, go search!
        if (searchRequest is { Latitude: not null, Longitude: not null })
        {
            var searchResult = await searchService.SearchAsync(searchRequest);
            return searchResult.response;
        }

        if (searchRequest.LocationPolygon != null && searchRequest.LocationPolygon.Count != 0)
        {
            var searchResult = await searchService.SearchAsync(searchRequest);
            return searchResult.response;
        }
        
        // If not, check to see if we have a location name passed
        if (string.IsNullOrWhiteSpace(searchRequest.Location))
        {
            throw new ArgumentException("You must enter either a latitude and longitude or a location");
        }

        // If we have a location, let's see if we can find the lat/long for it
        var locationResult = await searchService.GetGeoLocationAsync(searchRequest.Location);
        if (locationResult is { IsValid: true, Location: not null })
        {
            searchRequest.Latitude = locationResult.Location.Latitude;
            searchRequest.Longitude = locationResult.Location.Longitude;
        }
        else
        {
            throw new ArgumentException("The location you specified cannot be found");
        }

        var result = await searchService.SearchAsync(searchRequest);
        return result.response;
    }

    [McpServerTool, Description("Get the full details of a course by its instance ID")]
    public async Task<CourseDetailsResponse?> GetCourseById(
        [Description("The ID of the course to retrieve")]
        Guid instanceId)
    {
        return await courseService.GetCourseByInstanceIdAsync(instanceId);
    }
}