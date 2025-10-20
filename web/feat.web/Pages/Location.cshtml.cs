using System.ComponentModel.DataAnnotations;
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

        return Page();
    }

    public IActionResult OnPost()
    {
        Search = HttpContext.Session.Get<Search>("Search") ?? new Search();

        // If it's anything other than HE, we need to validate distance and location
        if (Search.SearchType != SearchType.HE)
        {
            if (string.IsNullOrEmpty(Location))
                ModelState.AddModelError("Location", "Please enter a location");
            if (!Distance.HasValue)
                ModelState.AddModelError("Distance", "Please select how far you would be happy to travel");
        }
        else
        {
            if (string.IsNullOrEmpty(Location))
                ModelState.AddModelError("Location", "Please enter a location or click \"Skip this step\"");
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