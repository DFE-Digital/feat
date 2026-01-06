using feat.api.Controllers;
using feat.api.Models;
using feat.api.Services;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using NUnit.Framework;

namespace feat.api.tests.Controllers;

[TestFixture]
public class SearchControllerTests
{
    private ISearchService _searchService;
    private SearchController _controller;

    [SetUp]
    public void Setup()
    {
        _searchService = Substitute.For<ISearchService>();
        _controller = new SearchController(_searchService);
    }

    [Test]
    public async Task Search_ReturnsOk_WhenValidationPasses()
    {
        var request = new SearchRequest { Query = "Art" };
        
        var response = new SearchResponse();
        
        _searchService
            .SearchAsync(request)
            .Returns((new ValidationResult(), response));
        
        var actionResult = await _controller.Search(request);
        
        var result = actionResult.Result as ObjectResult;
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.StatusCode, Is.EqualTo(200));
        Assert.That(result!.Value, Is.EqualTo(response));
    }

    [Test]
    public async Task Search_ReturnsBadRequest_WhenValidationFails()
    {
        var request = new SearchRequest { Query = "" };
        
        var validation = new ValidationResult();
        validation.AddError("Query", "Required field");
        
        _searchService
            .SearchAsync(request)
            .Returns((validation, null));

        var actionResult = await _controller.Search(request);
        
        var result = actionResult.Result as ObjectResult;
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.StatusCode, Is.EqualTo(400));
    }
}