using feat.web.Enums;
using feat.web.Extensions;
using feat.web.Models;
using feat.web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace feat.web.Pages.CourseDetails;

public class DetailsUniversityDegreeModel(ILogger<DetailsUniversityDegreeModel> logger, ISearchService searchService) : PageModel
{   
    [BindProperty]
    public string? CourseId { get; set; }
    
    
    public required Search Search { get; set; }
    
    public void OnGet(string? id)
    {
        logger.LogInformation("OnGet called");

        try
        {
            Search = HttpContext.Session.Get<Search>("Search") ?? new Search();
            Search.SetPage(PageName.DeatilsUniversityDegree);
            HttpContext.Session.Set("Search", Search);
            
            CourseId = !string.IsNullOrEmpty(id) ? id : "missing id";
            logger.LogInformation("CourseId: {Id}", id);
            
            // Get from server the Course Details - Api call
            var result = searchService.GetCourseDetails(CourseId);

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
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