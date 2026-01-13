using System.ComponentModel.DataAnnotations;
using feat.api.Models;
using feat.api.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace feat.api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AutocompleteLocationsController(ISearchService searchService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<AutoCompleteLocation[]>> Query(
        [FromQuery]
        [MinLength(3)]
        string query)
    {
        var locations = await searchService.GetAutoCompleteLocationsAsync(query);

        return Ok(locations);
    }
    
    [HttpPost]
    public async Task<ActionResult<bool>> Valid(
        [FromQuery]
        [MinLength(3)]
        string query)
    {
        var location = await searchService.GetGeoLocationAsync(query);

        return location is { IsValid: true };
    }
}