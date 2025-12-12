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
        var (validation, response) = await searchService.SearchAsync(request);

        if (!validation.IsValid)
        {
            return ValidationProblem(new ValidationProblemDetails(validation.Errors)
            {
                Title = "One or more validation errors occurred.",
                Status = StatusCodes.Status400BadRequest
            });
        }

        if (response == null)
        {
            return NotFound();
        }

        return Ok(response);
    }
}