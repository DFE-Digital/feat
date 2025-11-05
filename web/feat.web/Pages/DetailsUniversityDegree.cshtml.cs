using feat.web.Enums;
using feat.web.Extensions;
using feat.web.Models;
using feat.web.Models.ViewModels;
using feat.web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using feat.web.Utils;

namespace feat.web.Pages;

public class DetailsUniversityDegreeModel(ILogger<DetailsUniversityDegreeModel> logger, ISearchService searchService) : PageModel
{   
    [BindProperty]
    public string? CourseId { get; set; }
    
    [BindProperty]
    public CourseDetailsUniversity? UniversityCourseDetails { get; set; }
    
    public required Search Search { get; set; }
    
    public IActionResult OnGet(string id)
    {
        logger.LogInformation("OnGet called");

        try
        {
            if(string.IsNullOrEmpty(id))
                return RedirectToPage(PageName.LoadCourses); 
            
            Search = HttpContext.Session.Get<Search>("Search") ?? new Search();
            Search.CourseId = id;
            Search.SetPage(PageName.DetailsUniversityDegree);
            HttpContext.Session.Set("Search", Search);
            
            CourseId = !string.IsNullOrEmpty(id) ? id : "missing id";
            logger.LogInformation("CourseId: {Id}", id);
            
            // Get from server the Course Details - Api call
            var response = searchService.GetCourseDetails(Search, "").Result;
            UniversityCourseDetails = (CourseDetailsUniversity)(response.CourseDetails!);
            
            // Courses = searchResponse.SearchResults.ToCourses();

            return Page();
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
    
    public IActionResult Test()
    {
        return Redirect(Request.Headers["Referer"].ToString());
    }
    
}