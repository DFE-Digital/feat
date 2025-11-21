using feat.web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace feat.web.Pages;

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

    public void OnPost()
    {
        Response.Cookies.Append("analytics-cookies", "true");
        Response.Cookies.Append("marketing-cookies", "true");
    }
}