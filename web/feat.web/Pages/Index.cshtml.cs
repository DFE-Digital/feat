using feat.web.Extensions;
using feat.web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace feat.web.Pages;

public class IndexModel(ILogger<IndexModel> logger) : PageModel
{
    public IActionResult OnGet()
    {
        var search = new Search();
        search.SetPage(PageName.Index);
        HttpContext.Session.Set("Search", search);
        
        return Page();
    }
}