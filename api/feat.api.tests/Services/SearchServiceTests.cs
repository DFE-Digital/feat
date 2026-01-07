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
    private CourseDbContext _dbContext;
    private IFusionCache _cache;
    private SearchService _service;

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
            Query = "Art",
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
    
    [TearDown]
    public void TearDown()
    {
        _dbContext.Dispose();
    }
}
