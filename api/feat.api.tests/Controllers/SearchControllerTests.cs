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
    private ISearchService _searchService = null!;
    private SearchController _controller = null!;

    [SetUp]
    public void Setup()
    {
        _searchService = Substitute.For<ISearchService>();
        _controller = new SearchController(_searchService);
    }

    [Test]
    public async Task Search_ReturnsOk_WhenValidationPasses()
    {
        var request = new SearchRequest { Query = ["Art"] };
        
        var response = new SearchResponse();
        
        _searchService
            .SearchAsync(request)
            .Returns((new ValidationResult(), response));
        
        var result = await _controller.Search(request);
        
        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult, Is.Not.Null);
        Assert.That(okResult!.StatusCode, Is.EqualTo(200));
        Assert.That(okResult!.Value, Is.EqualTo(response));
    }

    [Test]
    public async Task Search_ReturnsBadRequest_WhenQueryMissing()
    {
        var request = new SearchRequest { Query = [] };
        _controller.ModelState.AddModelError(nameof(request.Query), "Required");

        var result = await _controller.Search(request);

        var objectResult = result.Result as ObjectResult;
        Assert.That(objectResult, Is.Not.Null);

        var problem = objectResult!.Value as ValidationProblemDetails;
        Assert.That(problem, Is.Not.Null);
        Assert.That(problem!.Errors, Is.Not.Empty);
        Assert.That(problem!.Errors.ContainsKey(nameof(SearchRequest.Query)));
    }

    [Test]
    public async Task Search_ReturnsBadRequest_WhenPageInvalid()
    {
        var request = new SearchRequest { Query = ["Art"], Page = 0 };
        _controller.ModelState.AddModelError(nameof(request.Page), "Page must be at least 1.");

        var result = await _controller.Search(request);

        var objectResult = result.Result as ObjectResult;
        Assert.That(objectResult, Is.Not.Null);

        var problem = objectResult!.Value as ValidationProblemDetails;
        Assert.That(problem, Is.Not.Null);
        Assert.That(problem!.Errors, Is.Not.Empty);
        Assert.That(problem!.Errors.ContainsKey(nameof(SearchRequest.Page)));
    }

    [Test]
    public async Task Search_ReturnsBadRequest_WhenPageSizeInvalid()
    {
        var request = new SearchRequest { Query = ["Art"], PageSize = 0 };
        _controller.ModelState.AddModelError(nameof(request.PageSize), "PageSize must be greater than 0.");

        var result = await _controller.Search(request);

        var objectResult = result.Result as ObjectResult;
        Assert.That(objectResult, Is.Not.Null);

        var problem = objectResult!.Value as ValidationProblemDetails;
        Assert.That(problem, Is.Not.Null);
        Assert.That(problem!.Errors, Is.Not.Empty);
        Assert.That(problem!.Errors.ContainsKey(nameof(SearchRequest.PageSize)));
    }
}