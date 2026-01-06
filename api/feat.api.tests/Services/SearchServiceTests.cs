using Azure.Search.Documents;
using feat.api.Data;
using feat.api.Models;
using feat.api.Services;
using feat.common.Configuration;
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
    private SearchService _service;
    private IFusionCache _cache;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<CourseDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        
        var dbContext = new CourseDbContext(options);

        var azureOptions = Substitute.For<IOptionsMonitor<AzureOptions>>();
        _cache = Substitute.For<IFusionCache>();

        _service = new SearchService(
            azureOptions,
            Substitute.For<SearchClient>(),
            Substitute.For<EmbeddingClient>(),
            dbContext,
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
}
