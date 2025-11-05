using feat.web.Extensions;
using feat.web.Models;
using feat.web.Models.ViewModels;
using feat.web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using feat.web.Utils;

namespace feat.web.Pages;

public class DetailsApprenticeshipModel(ILogger<DetailsApprenticeshipModel> logger, ISearchService searchService) : PageModel
{
    [BindProperty]
    public string? CourseId { get; set; }

    [BindProperty] 
    public CourseDetailsApprenticeship? ApprenticeshipCourseDetails { get; set; } 
    
    public required Search Search { get; set; }
    
    public async Task<IActionResult> OnGet(string? id)
    {
        logger.LogInformation("OnGet called");

        try
        {
            if(string.IsNullOrEmpty(id))
                return RedirectToPage(PageName.LoadCourses); 
            
            Search = HttpContext.Session.Get<Search>("Search") ?? new Search();
            Search.CourseId = id;
            Search.SetPage(PageName.DetailsApprenticeship);
            HttpContext.Session.Set("Search", Search);
            
            CourseId = !string.IsNullOrEmpty(id) ? id : "missing id";
            logger.LogInformation("CourseId: {id}", id);
            
            var response = await searchService.GetCourseDetails(Search, "");
            ApprenticeshipCourseDetails = (CourseDetailsApprenticeship)(response.CourseDetails!);
            
            return Page();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
}