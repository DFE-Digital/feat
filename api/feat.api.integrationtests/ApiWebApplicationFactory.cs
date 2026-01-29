using feat.api.Models;
using feat.api.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace feat.api.integrationtests;

public class ApiWebApplicationFactory : WebApplicationFactory<Program>
{
    public ISearchService SearchService { get; } = Substitute.For<ISearchService>();
    public ICourseService CourseService { get; } = Substitute.For<ICourseService>();
    
    public HttpClient CreateClientWithDefaults() => CreateClient();
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<ISearchService>();
            services.RemoveAll<ICourseService>();
            
            services.AddSingleton(SearchService);
            services.AddSingleton(CourseService);
            
            SearchService.SearchAsync(Arg.Any<SearchRequest>())!
                .Returns(callInfo =>
                {
                    var request = callInfo.Arg<SearchRequest>();
                    return Task.FromResult((new ValidationResult(), new SearchResponse
                    {
                        Courses = [],
                        Facets = [],
                        Page = request.Page,
                        PageSize = request.PageSize,
                        TotalCount = 0
                    }));
                });

            SearchService.GetAutoCompleteLocationsAsync(Arg.Any<string>())
                .Returns([]);

            SearchService.GetGeoLocationAsync(Arg.Any<string>())
                .Returns(new GeoLocationResponse(new GeoLocation(), true));
            
            CourseService.GetCourseByInstanceIdAsync(Arg.Any<Guid>())
                .Returns((CourseDetailsResponse?)null);
        });
    }
}
