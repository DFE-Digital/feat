using feat.web.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace feat.web.Pages.TrainingOptionsPages;

public class TrainingOptionsModel(StaticNavigationHandler staticNavigation) : PageModel
{
    public string? RefererUrl { get; private set; } = "";
    
    public void OnGet()
    {
        RefererUrl = staticNavigation.GetRefererUrl();
    }
}