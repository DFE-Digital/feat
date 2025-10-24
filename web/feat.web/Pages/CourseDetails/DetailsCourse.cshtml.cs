using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace feat.web.Pages.CourseDetails;

public class DetailsCourseModel(ILogger<DetailsCourseModel> logger) : PageModel
{
    [BindProperty]
    public string? CourseId { get; set; }
    
    public void OnGet(string courseId)
    {
        CourseId = courseId;
        
        logger.LogInformation("CourseId: {courseId}", courseId);
        // Get from server the Course Details
    }
 
    public IActionResult OnPost()
    {
        if (string.IsNullOrEmpty(CourseId))
        {
            logger.LogDebug("My data is {myData} doesnt exist", CourseId);
        }
        else logger.LogInformation("My data is {myData}",CourseId);

        return Page();
    }
}