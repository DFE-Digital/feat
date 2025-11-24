using feat.web.Services;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace feat.web.Pages.LinkPages;

public class Cookies : PageModel
{
    [BindProperty] 
    public string? AnalyticsCookie { get; set; }
    
    [BindProperty]
    public string? MarketingCookie { get; set; }
    
    public void OnGet()
    {
        var analyticsCookieValue = Request.Cookies["analytics-cookies"];
        var matketingCookieValue = Request.Cookies["marketing-cookies"];
        
        if (analyticsCookieValue != null)
        {
            AnalyticsCookie = analyticsCookieValue;
        }
        if (analyticsCookieValue != null)
        {
            MarketingCookie = matketingCookieValue;
        }
    }
}