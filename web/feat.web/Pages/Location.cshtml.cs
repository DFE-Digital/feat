using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using feat.web.Enums;
using feat.web.Extensions;
using feat.web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using feat.web.Utils;

namespace feat.web.Pages;

public class LocationModel(ILogger<LocationModel> logger) : PageModel
{

    [BindProperty]
    [MaxLength(100, ErrorMessage = SharedStrings.LessThan100Char)]
    public string? Location { get; set; }
    
    [BindProperty]
    public Distance? Distance { get; set; }
    
    public required Search Search { get; set; }
    
    public IActionResult OnGet()
    {
        Search = HttpContext.Session.Get<Search>("Search") ?? new Search();
        if (!Search.Updated)
            return RedirectToPage(PageName.Index); 
        
        // If you've come here from LoadCourses page then start to 'new Search'
        if (Search.History.Contains(PageName.LoadCourses))
        {
            Search = new Search();
            Search.SetPage(PageName.Index); 
        }

        if (Search.Distance.HasValue)
            Distance = Search.Distance;
        if (!string.IsNullOrEmpty(Search.Location))
            Location = Search.Location;

        Search.SetPage(PageName.Location); 
        HttpContext.Session.Set("Search", Search);
        
        return Page();
    }

    public IActionResult OnPost()
    {
        Search = HttpContext.Session.Get<Search>("Search") ?? new Search();

        // TODO validate entered location is real or entered post-code is real.
        
        var distanceValue = Distance.HasValue? Distance.Value : new Distance();
        if (!string.IsNullOrEmpty(Location) && distanceValue == 0)
        {
            ModelState.AddModelError("Distance", SharedStrings.SelectHowFarTravel);
        }
        
        if (!ModelState.IsValid)
            return Page();
        
        Search.Updated = true;
        
        if (!string.IsNullOrEmpty(Location)) 
            Search.Location = Location.Trim();
        if (Distance != null) 
            Search.Distance = Distance.Value;

        HttpContext.Session.Set("Search", Search);

        return RedirectToPage(PageName.Interests); 
    }
}