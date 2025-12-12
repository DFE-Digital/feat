using feat.api.Models;
using feat.api.Services;
using Microsoft.AspNetCore.Mvc;

namespace feat.api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SearchController(ISearchService searchService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<SearchResponse>> Search([FromBody] SearchRequest request)
    {
        var result = await searchService.SearchAsync(request);

        if (result?.Error != null)
        {
            return ValidationProblem(
                title: "Invalid location",
                detail: result.Error,
                statusCode: StatusCodes.Status400BadRequest
            );
        }

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }
}