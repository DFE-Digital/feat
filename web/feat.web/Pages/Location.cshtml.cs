using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using feat.web.Enums;
using feat.web.Extensions;
using feat.web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace feat.web.Pages;

public class LocationModel(ILogger<LocationModel> logger) : PageModel
{

    [BindProperty]
    public string? Location { get; set; }
    
    [BindProperty]
    public Distance? Distance { get; set; }
    
    public required Search Search { get; set; }
    
    public IActionResult OnGet()
    {
        Search = HttpContext.Session.Get<Search>("Search") ?? new Search();
        if (!Search.Updated)
            return RedirectToPage(PageName.Index); // ??
        
        if (Search.Distance.HasValue)
            Distance = Search.Distance;
        if (!string.IsNullOrEmpty(Search.Location))
            Location = Search.Location;

        Search.SetPage(PageName.Location); 
        HttpContext.Session.Set("Search", Search);
        
        //TODO If you've come here from LoadCourses page Need to 'new Search'

        return Page();
    }

    public IActionResult OnPost()
    {
        Search = HttpContext.Session.Get<Search>("Search") ?? new Search();

        // If it's anything other than HE, we need to validate distance and location

        if (!string.IsNullOrEmpty(Location.Trim()))
        {
            // Has to be a full-name or a full-postcode.
            // Use an external service to validate - legitimate postcode or real-location, city, town, village
            
            //^([A-Z]{1,2}\d[A-Z\d]? ?\d[A-Z]{2}|GIR ?0A{2})$
            if (Regex.Match(Location, @"^([A-Z]{1,2}\d[A-Z\d]? ?\d[A-Z]{2}|GIR ?0A{2})$").Success == false)
            {
                // Does not match Postcode.
                //ModelState.AddModelError("Location", "Please enter a valid postcode");
            }
            else
            {
                // Is what's entered a real location name, if not.
                // ModelState.AddModelError("Location", "Please enter a valid location");
            }
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