using System.Text.RegularExpressions;
using Azure.Search.Documents;
using feat.api.Data;
using feat.api.Models;
using feat.api.Services;
using feat.common.Configuration;
using feat.common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NSubstitute;
using NUnit.Framework;
using OpenAI.Embeddings;
using ZiggyCreatures.Caching.Fusion;

namespace feat.api.tests.Services;

[TestFixture]
public class SearchServiceTests
{
    private CourseDbContext _dbContext = null!;
    private IFusionCache _cache = null!;
    private SearchService _service = null!;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<CourseDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        
        _dbContext = new CourseDbContext(options);
        _cache = Substitute.For<IFusionCache>();
        
        _service = new SearchService(
            Substitute.For<IOptionsMonitor<AzureOptions>>(),
            Substitute.For<SearchClient>(),
            Substitute.For<EmbeddingClient>(),
            _dbContext,
            _cache
        );
    }

    [Test]
    public async Task SearchAsync_ReturnsValidationError_WhenLocationInvalid()
    {
        const string invalidLocation = "Nowhere";

        var request = new SearchRequest
        {
            Query = ["Art"],
            Location = invalidLocation
        };

        _cache.GetOrSetAsync(
            Arg.Is<string>(k => k == $"location:{invalidLocation}"),
            Arg.Any<Func<FusionCacheFactoryExecutionContext<GeoLocationResponse>, CancellationToken, Task<GeoLocationResponse>>>(),
            Arg.Any<FusionCacheEntryOptions>(),
            Arg.Any<CancellationToken>()
        ).Returns(callInfo =>
        {
            var factory = callInfo.ArgAt<Func<FusionCacheFactoryExecutionContext<GeoLocationResponse>, CancellationToken, Task<GeoLocationResponse>>>(1);
            var task = factory(null!, CancellationToken.None);
            return new ValueTask<GeoLocationResponse>(task);
        });

        var (validation, response) = await _service.SearchAsync(request);

        Assert.That(validation.IsValid, Is.False);
        Assert.That(validation.Errors.ContainsKey("location"));
        Assert.That(response, Is.Null);
    }
    
    [Test]
    public async Task GetAutoCompleteLocationsAsync_PrioritizesExactMatch()
    {
        _dbContext.LookupLocations.AddRange(
            new LocationLatLong
            {
                Name = "Manchester Piccadilly",
                CleanName = "manchester piccadilly",
                Latitude = 1.1d,
                Longitude = 2.1d
            },
            new LocationLatLong
            {
                Name = "Manchester",
                CleanName = "manchester",
                Latitude = 1.0d,
                Longitude = 2.0d
            }
        );
        
        await _dbContext.SaveChangesAsync();
        
        var results = await _service.GetAutoCompleteLocationsAsync("Manchester");
        
        Assert.That(results[0].Name, Is.EqualTo("Manchester"));
    }
    
    [Test]
    public void SearchAsync_CalculateDistance_CalculatesMilesCorrectly()
    {
        var course = new Course
        {
            Id = default,
            InstanceId = default,
            Title = "Fine Art",
            Location = new GeoLocation
            {
                Latitude = 53.4808, // Manchester
                Longitude = -2.2426
            }
        };

        var userLocation = new GeoLocation
        {
            Latitude = 51.5074, // London
            Longitude = -0.1278
        };

        course.CalculateDistance(userLocation);

        Assert.That(course.Distance, Is.EqualTo(163).Within(2));
    }
    
    [Test]
    public void ApplySearchOrdering_PreservesAiSearchResultOrder()
    {
        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();

        var dbCourses = new List<Course>
        {
            new()
            {
                Id = default,
                InstanceId = id2,
                Title = "Questionable Art"
            },
            new()
            {
                Id = default,
                InstanceId = id1,
                Title = "Fine Art"
            }
        };

        var aiSearchResults = new List<AiSearchResult>
        {
            new()
            {
                Id = default,
                InstanceId = id1,
                RerankerScore = 2.0
            },
            new()
            {
                Id = default,
                InstanceId = id2,
                RerankerScore = 1.9
            }
        };

        var ordered = SearchService.ApplySearchOrdering(dbCourses, aiSearchResults);

        Assert.That(ordered[0].InstanceId, Is.EqualTo(id1));
        Assert.That(ordered[1].InstanceId, Is.EqualTo(id2));
    }
    
    [Test]
    public async Task SearchAsync_ReturnsValidationError_WhenLocationHasNoGeoLocation()
    {
        _dbContext.LookupLocations.Add(new LocationLatLong
        {
            Name = "Somewhere",
            CleanName = "somewhere",
            Latitude = null,
            Longitude = null
        });

        await _dbContext.SaveChangesAsync();

        var request = new SearchRequest
        {
            Query = [ "Art" ],
            Location = "Somewhere"
        };
        
        _cache.GetOrSetAsync(
            Arg.Is<string>(k => k == $"location:Somewhere"),
            Arg.Any<Func<FusionCacheFactoryExecutionContext<GeoLocationResponse>, CancellationToken, Task<GeoLocationResponse>>>(),
            Arg.Any<FusionCacheEntryOptions>(),
            Arg.Any<CancellationToken>()
        ).Returns(callInfo =>
        {
            var factory = callInfo.ArgAt<Func<FusionCacheFactoryExecutionContext<GeoLocationResponse>, CancellationToken, Task<GeoLocationResponse>>>(1);
            var task = factory(null!, CancellationToken.None);
            return new ValueTask<GeoLocationResponse>(task);
        });

        var (validation, response) = await _service.SearchAsync(request);

        Assert.That(validation.IsValid, Is.False);
        Assert.That(validation.Errors.ContainsKey("location"));
        Assert.That(response, Is.Null);
    }
    
    [Test]
    public async Task SearchAsync_ReturnsValidationError_WhenRadiusInvalid()
    {
        _dbContext.LookupLocations.Add(new LocationLatLong
        {
            Name = "Manchester",
            CleanName = "manchester",
            Latitude = 53.4808,
            Longitude = -2.2426
        });
        
        await _dbContext.SaveChangesAsync();

        var request = new SearchRequest
        {
            Query = [ "Art" ],
            Location = "Manchester",
            Radius = 0
        };

        var (validation, _) = await _service.SearchAsync(request);

        Assert.That(validation.IsValid, Is.False);
        Assert.That(validation.Errors.ContainsKey("radius"));
    }
    
    [Test]
    public void BuildFilterExpression_WithFacetsAndLocation_BuildsExpectedFilter()
    {
        var request = new SearchRequest
        {
            Query = [ "Art" ],
            Radius = 10,
            CourseType = [ "Apprenticeship" ],
            StudyTime = [ "FullTime" ]
        };

        var location = new GeoLocation
        {
            Latitude = 51,
            Longitude = -1
        };

        var filter = SearchService.BuildFilterExpression(request, location);
        var normalized = Normalize(filter!);

        Assert.That(
            normalized,
            Is.EqualTo(
                "(CourseType eq 'Apprenticeship') and " +
                "(StudyTime eq 'FullTime') and " +
                "(geo.distance(Location, geography'POINT(-1 51)') le 16.0934 " +
                "or (LearningMethod eq 'Online' and Location eq null) " +
                "or IsNational eq true)"
            )
        );
    }
    
    [TearDown]
    public void TearDown()
    {
        _dbContext.Dispose();
    }
    
    private static string Normalize(string filter) => Regex
            .Replace(filter.ReplaceLineEndings(" ").Trim(), @"\s+", " ")
            .Replace("( ", "(")
            .Replace(" )", ")");
}
