using feat.web.Models.ViewModels;
using feat.web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace feat.web.Pages;

public class CourseModel(ISearchService searchService) : PageModel
{
    public CourseDetailsBase? Course { get; private set; }

    public async Task<IActionResult> OnGet(Guid id)
    {
        Course = await searchService.GetCourseDetails(id.ToString());

        if (Course == null)
        {
            return NotFound();
        }

        return Page();
    }
}