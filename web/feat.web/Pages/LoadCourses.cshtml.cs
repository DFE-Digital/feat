using feat.web.Extensions;
using feat.web.Models;
using feat.web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace feat.web.Pages;

public class LoadCoursesModel(ISearchService searchService, ILogger<LoadCoursesModel> logger) : PageModel
{
    public required Search Search { get; set; }
    
    public SearchResponse? SearchResponse { get; set; }
    
    public async Task<IActionResult> OnGetAsync([FromQuery] bool debug = false)
    {
        logger.LogInformation("OnGetAsync called");
        try
        {
            Search = HttpContext.Session.Get<Search>("Search") ?? new Search();
            if (!Search.Updated)
            {
                return RedirectToPage(PageName.Index);
            }
            
            Search.Debug = debug;
            Search.SetPage(PageName.LoadCourses);
            HttpContext.Session.Set("Search", Search);

            SearchResponse = await searchService.Search(Search, HttpContext.Session.Id);
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
        }
        return Page();
    }
}