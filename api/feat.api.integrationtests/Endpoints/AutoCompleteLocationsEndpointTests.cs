using feat.api.Models;
using NSubstitute;
using NUnit.Framework;
using System.Net;
using System.Net.Http.Json;

namespace feat.api.integrationtests.Endpoints;

[TestFixture]
public class AutoCompleteLocationsEndpointTests
{
    private const string Endpoint = "/api/autocompletelocations";
    
    private ApiWebApplicationFactory _factory = null!;
    private HttpClient _client = null!;

    [SetUp]
    public void Setup()
    {
        _factory = new ApiWebApplicationFactory();
        _client = _factory.CreateClientWithDefaults();
    }

    [Test]
    public async Task Get_Query_ReturnsResultsWhenDataAvailable()
    {
        const string query = "Man";

        _factory.SearchService.GetAutoCompleteLocationsAsync(query)
            .Returns([ new AutoCompleteLocation { Name = "Manchester", Latitude = 1.0, Longitude = 2.0 } ]);

        var response = await _client.GetAsync($"{Endpoint}?query={query}");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var locations = await response.Content.ReadFromJsonAsync<AutoCompleteLocation[]>();
        Assert.That(locations, Is.Not.Null);
        Assert.That(locations![0].Name, Is.EqualTo("Manchester"));
    }

    [Test]
    public async Task Post_Valid_ReturnsTrueWhenLocationValid()
    {
        const string postcode = "M1 7AB";

        _factory.SearchService.GetGeoLocationAsync(postcode)
            .Returns(new GeoLocationResponse(new GeoLocation(), true));

        var response = await _client.PostAsync($"{Endpoint}?query={postcode}", null);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var valid = await response.Content.ReadFromJsonAsync<bool>();
        Assert.That(valid, Is.True);
    }
}