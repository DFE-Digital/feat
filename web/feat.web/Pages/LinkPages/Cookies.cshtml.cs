using feat.web.Services;
using feat.web.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace feat.web.Pages.LinkPages;

public class Cookies(StaticNavigationHandler staticNavigation) : PageModel
{
    [BindProperty] 
    public string? AnalyticsCookie { get; set; }
    
    [BindProperty]
    public string? MarketingCookie { get; set; }
    
    public string? RefererUrl { get; private set; } = "";
    
    public IActionResult OnGet()
    {
        RefererUrl = staticNavigation.GetRefererUrl();
        
        AnalyticsCookie = Request.Cookies[SharedStrings.AnalyticsCookie];
        MarketingCookie = Request.Cookies[SharedStrings.MarketingCookie];
        
        return Page();
    }

    public IActionResult OnPost()
    {
        if (AnalyticsCookie != null)
        {
            Response.Cookies.Append(SharedStrings.AnalyticsCookie, AnalyticsCookie, new CookieOptions
            {
                Expires= DateTime.Now.AddYears(1)
            });
        }

        if (MarketingCookie != null)
        {
            Response.Cookies.Append(SharedStrings.MarketingCookie, MarketingCookie, new CookieOptions
            {
                Expires = DateTime.Now.AddYears(1)
            });
        }
        
        return RedirectToPage();
    }
}