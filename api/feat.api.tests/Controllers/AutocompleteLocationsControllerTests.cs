using feat.api.Controllers;
using feat.api.Models;
using feat.api.Services;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NUnit.Framework;

namespace feat.api.tests.Controllers;

[TestFixture]
public class AutocompleteLocationsControllerTests
{
    private ISearchService _searchService = null!;
    private AutocompleteLocationsController _controller = null!;

    [SetUp]
    public void Setup()
    {
        _searchService = Substitute.For<ISearchService>();
        _controller = new AutocompleteLocationsController(_searchService);
    }

    [Test]
    public async Task Query_ReturnsResults_WhenServiceHasData()
    {
        var locations = new[] { new AutoCompleteLocation { Name = "Manchester", Latitude = 1.0d, Longitude = 2.0d } };
        
        _searchService
            .GetAutoCompleteLocationsAsync("Man")
            .Returns(locations);

        var result = await _controller.Query("Man");
        
        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult!.StatusCode, Is.EqualTo(200));
        Assert.That(okResult!.Value, Is.EqualTo(locations));
    }

    [Test]
    public async Task Valid_ReturnsTrue_WhenLocationIsValid()
    {
        _searchService
            .GetGeoLocationAsync("M1 7AB")
            .Returns(new GeoLocationResponse(new GeoLocation(), true));

        var result = await _controller.Valid("M1 7AB");
        
        Assert.That(result.Value, Is.True);
    }
}