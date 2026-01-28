using feat.api.Models;
using NSubstitute;
using NUnit.Framework;
using System.Net;
using System.Net.Http.Json;

namespace feat.api.integrationtests.Endpoints;

[TestFixture]
public class SearchEndpointTests
{
    private const string Endpoint = "/api/search";
    
    private ApiWebApplicationFactory _factory = null!;
    private HttpClient _client = null!;

    [SetUp]
    public void Setup()
    {
        _factory = new ApiWebApplicationFactory();
        _client = _factory.CreateClientWithDefaults();
    }

    [Test]
    public async Task Post_Search_Returns200WhenValidRequest()
    {
        var request = new SearchRequest { Query = [ "Art" ] };
        
        _factory.SearchService.SearchAsync(Arg.Any<SearchRequest>())!
            .Returns(Task.FromResult((new ValidationResult(), new SearchResponse())));

        var response = await _client.PostAsJsonAsync(Endpoint, request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test]
    public async Task Post_Search_Returns400WhenQueryMissing()
    {
        var request = new SearchRequest { Query = null! };

        var response = await _client.PostAsJsonAsync(Endpoint, request);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
    }
}