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
    
    private SearchController _searchController;

    [SetUp]
    public void Setup()
    {
        _searchService = Substitute.For<ISearchService>();
        
        _searchController = new SearchController(_searchService);
    }

    [Test]
    public async Task WhenSearchReturnsResults_ShouldReturnOkResult()
    {
        var searchRequest = new SearchRequest
        {
            Query = "art",
            Location = "Manchester",
        };
        var validationResult = new ValidationResult();
        var searchResponse = new SearchResponse();
        _searchService.SearchAsync(searchRequest).Returns((validationResult, searchResponse));
        
        var result = await _searchController.Search(searchRequest);
        
        Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
        var okResult = result.Result as OkObjectResult;
        Assert.That(okResult?.Value, Is.EqualTo(searchResponse));
    }
}