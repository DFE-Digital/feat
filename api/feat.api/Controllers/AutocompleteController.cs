using System.ComponentModel.DataAnnotations;
using feat.api.Models;
using feat.api.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace feat.api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AutocompleteController(ISearchService searchService) : ControllerBase
{
    [HttpGet("{query}")]
    public async Task<ActionResult<AutoCompleteLocation[]>> Query(
        [MaxLength(100)]
        [MinLength(3)]
        string query)
    {
        var locations = await searchService.GetAutoCompleteLocationsAsync(query);

        return Ok(locations);
    }
}