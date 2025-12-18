using System.ComponentModel.DataAnnotations;
using feat.web.Enums;
using feat.web.Extensions;
using feat.web.Models;
using feat.web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using feat.web.Utils;

namespace feat.web.Pages;

public class LocationModel(ISearchService searchService, ILogger<LocationModel> logger) : PageModel
{

    [BindProperty]
    public string? Location { get; set; }
    
    [BindProperty]
    public Distance? Distance { get; set; }
    
    public required Search Search { get; set; }
    
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

        // TODO validate entered location is real or entered post-code is real.
        
        var distanceValue = Distance.HasValue? Distance.Value : new Distance();
        
        if (!string.IsNullOrEmpty(Location) && distanceValue == 0)
        {
            ModelState.AddModelError("Distance", SharedStrings.SelectHowFarCanUTravel);
        }
        if (string.IsNullOrEmpty(Location) && distanceValue > 0)
        {
            ModelState.AddModelError("Location", SharedStrings.EnterCityOrPostcode);
        }

        if (!ModelState.IsValid)
            return Page();
        
        Search.Updated = true;
        
        if (!string.IsNullOrEmpty(Location)) 
            Search.Location = Location.Trim();
        if (Distance != null) 
            Search.Distance = Distance.Value;
        
        Search.OriginalDistance = Distance;

        HttpContext.Session.Set("Search", Search);

        if (Search.VisitedCheckAnswers)
        {
            bool mustAnswerInterests = Distance == null || Distance == Enums.Distance.ThirtyPlus;
            if (!mustAnswerInterests) 
                return RedirectToPage(PageName.CheckAnswers);

            var hasInterests = Search.Interests.Any(searchInterest => !string.IsNullOrEmpty(searchInterest));
            return RedirectToPage(hasInterests ? PageName.CheckAnswers : PageName.Interests);
        }
        else
        {
            return RedirectToPage(PageName.Interests);
        }
    }
}