using feat.web.Models.ViewModels;
using feat.web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace feat.web.Pages;

public class CourseModel(ISearchService searchService, StaticNavigationHandler staticNavigation) : PageModel
{
    public CourseDetailsBase? Course { get; private set; }
    
    public string? RefererUrl { get; private set; } = "";

    public async Task<IActionResult> OnGet(Guid instanceId)
    {
        RefererUrl = staticNavigation.GetRefererUrl();
        
        Course = await searchService.GetCourseDetails(instanceId.ToString());

        if (Course == null)
        {
            return NotFound();
        }
        
        return Page();
    }
}