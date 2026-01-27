using feat.api.Services;
using feat.api.Models;
using Microsoft.AspNetCore.Mvc;

namespace feat.api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CoursesController(ICourseService courseService) : ControllerBase
{
    [HttpGet("{instanceId:guid}")]
    public async Task<ActionResult<CourseDetailsResponse>> GetCourseByInstanceId(Guid instanceId)
    {
        var course = await courseService.GetCourseByInstanceIdAsync(instanceId);

        if (course == null)
        {
            return NotFound();
        }

        return Ok(course);
    }
}