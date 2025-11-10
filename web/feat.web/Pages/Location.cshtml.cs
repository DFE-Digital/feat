using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using feat.web.Enums;
using feat.web.Extensions;
using feat.web.Models;
using feat.web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using feat.web.Utils;

namespace feat.web.Pages;

public partial class LocationModel(ISearchService searchService, ILogger<LocationModel> logger) : PageModel
{
    [BindProperty] 
    [MaxLength(100, ErrorMessage = SharedStrings.LessThan100Char)]
    public string? Location { get; set; }
    
    [BindProperty]
    public Distance? Distance { get; set; }
    
    public required Search Search { get; set; }
    
    [GeneratedRegex(SharedStrings.PostcodePattern)]
    private static partial Regex PostcodeRegex();

    public List<string> Locations { get; set; } = [];
    
    public async Task<IActionResult> OnGetAsync()
    {
        logger.LogInformation("OnGet");
        
        Search = HttpContext.Session.Get<Search>("Search") ?? new Search();
        if (!Search.Updated)
            return RedirectToPage(PageName.Index); 
        
        // If you've come here from LoadCourses page then start to 'new Search'
        if (Search.History.Contains(PageName.LoadCourses))
        {
            Search = new Search();
            Search.SetPage(PageName.Index); 
        }

        Locations = GetLocations();
        // Set up full list of facets;
        var searchResponse = await searchService.Search(Search, HttpContext.Session.Id);
        if (searchResponse.Facets.Any())
        {
            HttpContext.Session.Set(SharedStrings.AllClientFacets, searchResponse.Facets.ToClientFacets());    
        }

        if (!string.IsNullOrEmpty(Search.Location))
            Location = Search.Location;

        if (Search.Distance.HasValue)
            Distance = Search.Distance;
        
        Search.Updated = true;
        Search.SetPage(PageName.Location); 
        HttpContext.Session.Set("Search", Search);
        
        return Page();
    }

    public IActionResult OnPost()
    {
        Search = HttpContext.Session.Get<Search>("Search") ?? new Search();

        var location = Location?.Trim();
        var distanceValue = Distance.HasValue? Distance.Value : new Distance();
        if (!string.IsNullOrEmpty(location) && distanceValue == 0)
        {
            ModelState.AddModelError("Distance", SharedStrings.SelectHowFarUCanTravel);
        }

        if (!string.IsNullOrEmpty(location))
        {
            var locationIsValid = SharedStrings.LocationsInEngland.Contains(location);

            if (!locationIsValid)
            {
                // location does not match, check if it's a post-code 
                Regex postcodeRegex = PostcodeRegex();
                if (!postcodeRegex.IsMatch(location))
                {
                    // TODO autocomplete field is not working
                    ModelState.AddModelError("Location", SharedStrings.EnterValidLocation);
                }
            }
        }
        
        Locations = GetLocations();
        
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

    private static List<string> GetLocations()
    {
        return SharedStrings.LocationsInEngland.ToList();
    }

}