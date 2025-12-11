using feat.web.Extensions;
using feat.web.Models;
using feat.web.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace feat.web.Pages;

public class NoResultsSearchModel(ILogger<NoResultsSearchModel> logger) : PageModel
{
    public required Search Search { get; set; }
    
    public IActionResult OnGet()
    {
        logger.LogInformation("OnGet called");

        try
        {
            Search = HttpContext.Session.Get<Search>("Search") ?? new Search();
            Search.SetPage(PageName.NoResultsSearch);
            HttpContext.Session.Set("Search", Search);
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
        }
        return Page();
    }
    
}