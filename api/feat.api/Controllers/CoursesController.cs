using feat.api.Services;
using feat.api.Models;
using Microsoft.AspNetCore.Mvc;

namespace feat.api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CoursesController(ICourseService courseService) : ControllerBase
{
    [HttpGet("{courseId:guid}")]
    public async Task<ActionResult<CourseDetailsResponse>> GetCourseById(Guid courseId)
    {
        var course = await courseService.GetCourseByIdAsync(courseId);

        if (course == null)
        {
            return NotFound();
        }

        return Ok(course);
    }
}