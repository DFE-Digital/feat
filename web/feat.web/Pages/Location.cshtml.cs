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
            return RedirectToPage("Index");
        
        if (Search.Distance.HasValue)
            Distance = Search.Distance;
        if (!string.IsNullOrEmpty(Search.Location))
            Location = Search.Location;
        
        Search.SetPage("Location");
        HttpContext.Session.Set("Search", Search);
        
<<<<<<< Updated upstream
<<<<<<< Updated upstream
=======
=======
>>>>>>> Stashed changes
        //TODO If you've come here from LoadCourses page Need to 'new Search'
        //

>>>>>>> Stashed changes
        return Page();
    }

    public IActionResult OnPost()
    {
        Search = HttpContext.Session.Get<Search>("Search") ?? new Search();

        // If its anything other than HE, we need to validate distance and location
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
        
        if (!string.IsNullOrEmpty(Location)) Search.Location = Location;
        if (Distance != null) Search.Distance = Distance.Value;


        HttpContext.Session.Set("Search", Search);

        if (Search.AgeGroup is AgeGroup.UnderEighteen && Search.SearchType is SearchType.FE or SearchType.Return)
        {
            return RedirectToPage("How");
        }
        
        if (Search.Interests.Any() || Search.Subjects.Any() || Search.Careers.Any())
            return RedirectToPage("Summary");

        return RedirectToPage("Interests");
    }
}