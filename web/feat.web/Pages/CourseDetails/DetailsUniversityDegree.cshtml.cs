using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace feat.web.Pages.CourseDetails;

public class DetailsUniversityDegreeModel(ILogger<DetailsUniversityDegreeModel> logger) : PageModel
{
    [BindProperty]
    public string? CourseId { get; set; }
    
    public void OnGet(string courseId)
    {
        CourseId = courseId;
        
        logger.LogInformation("CourseId: {courseId}", courseId);
        // Get from server the Course Details
    }

    
    public IActionResult OnPost(string courseId)
    {
        logger.LogInformation("My data is {myData}",courseId);
        
        if (string.IsNullOrEmpty(courseId))
        {
            logger.LogDebug("My data is {myData}",courseId);
        }

        return RedirectToPage("./University");
    }
}