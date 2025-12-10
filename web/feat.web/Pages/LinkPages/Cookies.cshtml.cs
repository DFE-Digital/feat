using feat.web.Services;
using Microsoft.AspNetCore.Components.Routing;
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
        
        AnalyticsCookie = Request.Cookies[nameof(AnalyticsCookie)];
        MarketingCookie = Request.Cookies[nameof(MarketingCookie)];
        
        return Page();
    }

    public IActionResult OnPost()
    {
        if (AnalyticsCookie != null)
        {
            Response.Cookies.Append(nameof(AnalyticsCookie), AnalyticsCookie, new CookieOptions
            {
                Expires= DateTime.Now.AddDays(1)
            });
        }

        if (MarketingCookie != null)
        {
            Response.Cookies.Append(nameof(MarketingCookie), MarketingCookie, new CookieOptions
            {
                Expires = DateTime.Now.AddDays(1)
            });
        }
        
        return RedirectToPage();
    }
}