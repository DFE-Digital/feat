using System.ComponentModel.DataAnnotations;
using feat.web.Enums;
using feat.web.Extensions;
using feat.web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace feat.web.Pages;

public class LocationModel(ILogger<LocationModel> logger) : PageModel
{

    public void OnGet()
    {
        Search = HttpContext.Session.Get<Search>("Search") ?? new Search();
        if (Search.Distance.HasValue)
            Distance = Search.Distance;
        if (!string.IsNullOrEmpty(Search.Location))
            Location = Search.Location;
    }
    
    [BindProperty]
    [Required(ErrorMessage = "Please enter a location, or click \"Skip this step\"")]
    public string? Location { get; set; }
    
    [BindProperty]
    [Required(ErrorMessage = "Please select how far you would be happy to travel, or click \"Skip this step\"")]
    public Distance? Distance { get; set; }
    
    public required Search Search { get; set; }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
            return Page();
        
        Search = HttpContext.Session.Get<Search>("Search") ?? new Search();
        
        if (!string.IsNullOrEmpty(Location)) Search.Location = Location;
        if (Distance != null) Search.Distance = Distance.Value;


        HttpContext.Session.Set("Search", Search);

        return RedirectToPage("How");
    }
}