using feat.web.Extensions;
using feat.web.Models;
using feat.web.Models.ViewModels;
using feat.web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using feat.web.Utils;

namespace feat.web.Pages;

public class DetailsCourseModel(ILogger<DetailsCourseModel> logger, ISearchService searchService) : PageModel
{
    [BindProperty]
    public string? CourseId { get; set; }

    [BindProperty] 
    public CourseDetailsCourse? CourseDetails { get; set; } 
    
    public required Search Search { get; set; }
    
    public async Task<IActionResult> OnGetAsync(string id)
    {
        logger.LogInformation("OnGet called");
        try
        {   
            if(string.IsNullOrEmpty(id))
                return RedirectToPage(PageName.LoadCourses); 
            
            Search = HttpContext.Session.Get<Search>("Search") ?? new Search();
            Search.CourseId = id;
            Search.SetPage(PageName.DetailsCourse);
            HttpContext.Session.Set("Search", Search);
            
            CourseId = !string.IsNullOrEmpty(id) ? id : "missing id";
            logger.LogInformation("CourseId: {id}", id);
            
            var response = await searchService.GetCourseDetails(Search, "");
            CourseDetails = (CourseDetailsCourse)(response.CourseDetails!);
            
            return Page();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
 
    public IActionResult OnPost()
    {
        logger.LogInformation("OnPost called");

        return Page();
    }
}