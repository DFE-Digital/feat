using System.ComponentModel.DataAnnotations;
using feat.web.Enums;
using feat.web.Extensions;
using feat.web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace feat.web.Pages;

public class SituationModel (ILogger<SituationModel> logger) : PageModel
{
    
    [BindProperty]
    [Required(ErrorMessage = "Please select your age group or select \"Skip this step\"")]
    public AgeGroup? AgeGroup { get; set; }
    
    [BindProperty]
    [Required(ErrorMessage = "Please select what you want to do or select \"Skip this step \"")]
    public SearchType? SearchType { get; set; }
    
    public required Search Search { get; set; }
    
    public void OnGet()
    {
        Search = HttpContext.Session.Get<Search>("Search") ?? new Search();
        if (Search.AgeGroup.HasValue)
            AgeGroup = Search.AgeGroup;
        if (Search.SearchType.HasValue)
            SearchType = Search.SearchType;
        
        Search.SetPage("Situation");
        HttpContext.Session.Set("Search", Search);
        
    }

    public IActionResult OnPost()
    {
        Search = HttpContext.Session.Get<Search>("Search") ?? new Search();
        if (!ModelState.IsValid)
            return Page();
        
        Search.SetPage("Situation");
        Search.Updated = true;
        
        if (AgeGroup.HasValue) Search.AgeGroup = AgeGroup.Value;
        if (SearchType.HasValue) Search.SearchType = SearchType.Value;

        HttpContext.Session.Set("Search", Search);
/*
        if (AgeGroup is Enums.AgeGroup.UnderEighteen or Enums.AgeGroup.EighteenToTwentyFour 
            && SearchType == Enums.SearchType.HE)
        {
            return RedirectToPage("Interests");
        }*/

        if (AgeGroup == Enums.AgeGroup.UnderEighteen 
            && SearchType is Enums.SearchType.FE or Enums.SearchType.Return)
        {
            return RedirectToPage("Location");
        }

        return RedirectToPage("How");
        
    }
}