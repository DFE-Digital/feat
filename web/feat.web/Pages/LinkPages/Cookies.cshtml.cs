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
    
    public void OnGet()
    {
        RefererUrl = staticNavigation.GetRefererUrl();
        
        
        var analyticsCookieValue = Request.Cookies["analytics-cookies"];
        var matketingCookieValue = Request.Cookies["marketing-cookies"];
        
        if (analyticsCookieValue != null)
        {
            AnalyticsCookie = analyticsCookieValue;
        }
        if (matketingCookieValue != null)
        {
            MarketingCookie = matketingCookieValue;
        }
    }
}