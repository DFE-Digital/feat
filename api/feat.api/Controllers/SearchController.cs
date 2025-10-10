using feat.api.Models;
using feat.api.Services;
using Microsoft.AspNetCore.Mvc;

namespace feat.api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SearchController(ISearchService searchService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<SearchResponse>> Search([FromBody] SearchRequest? request)
    {
        if (request == null)
        {
            return BadRequest("Request body is required.");
        }

        var result = await searchService.SearchAsync(request);

        if (result == null)
        {
            return NotFound();
        }
        
        return Ok(result);
    }
}