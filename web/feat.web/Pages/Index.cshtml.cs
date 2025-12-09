using feat.web.Extensions;
using feat.web.Models;
using feat.web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using feat.web.Utils;

namespace feat.web.Pages;

public class IndexModel(StaticNavigationHandler staticNavigation,ILogger<IndexModel> logger) : PageModel
{
    public IActionResult OnGet()
    {
        var search = new Search();
        search.SetPage(PageName.Index);
        staticNavigation.Initialise();
        HttpContext.Session.Set("Search", search);
        
        return Page();
    }
}