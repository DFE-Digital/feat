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
        Search = HttpContext.Session.Get<Search>("Search") ?? new Search();
        
        if (!Search.Updated)
        {
            return RedirectToPage(PageName.Index);
        }
        
        Search.SetPage(PageName.NoResultsSearch);
        HttpContext.Session.Set("Search", Search);
        
        return Page();
    }
}