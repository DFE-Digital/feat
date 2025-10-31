using System.ComponentModel.DataAnnotations;
using feat.web.Enums;
using feat.web.Extensions;
using feat.web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using feat.web.Utils;

namespace feat.web.Pages;

public class InterestsModel(ILogger<InterestsModel> logger) : PageModel
{
    [BindProperty]
    [MaxLength(100, ErrorMessage = SharedStrings.LessThan100Char)]
    public string? UserInterest1 { get; set; } = string.Empty;
    
    [BindProperty]
    [MaxLength(100, ErrorMessage = SharedStrings.LessThan100Char)]
    public string? UserInterest2 { get; set; }
    
    [BindProperty]
    [MaxLength(100, ErrorMessage = SharedStrings.LessThan100Char)]
    public string? UserInterest3 { get; set; }
    
    [BindProperty] 
    public bool FirstOptionMandatory { get; set; } = false;
    
    public required Search Search { get; set; }

    public IActionResult OnGet()
    {
        logger.LogInformation("OnGet");
        
        Search = HttpContext.Session.Get<Search>("Search") ?? new Search();

        if (!Search.Updated)
            return RedirectToPage(PageName.Index);

        // Populate the three individual interest fields
        if (Search.Interests.Count > 0 &&
            !string.IsNullOrEmpty(Search.Interests[0]))
        {
            UserInterest1 = Search.Interests[0];
        }
        if (Search.Interests.Count > 1 &&
            !string.IsNullOrEmpty(Search.Interests[1]))
        {
            UserInterest2 = Search.Interests[1];
        }
        if (Search.Interests.Count > 2 &&
            !string.IsNullOrEmpty(Search.Interests[2]))
        {
            UserInterest3 = Search.Interests[2];
        }

        if (string.IsNullOrEmpty(Search.Location) ||
            Search.Distance == null || Search.Distance == Distance.ThirtyPlus)
        {
            FirstOptionMandatory = true;
        }

        Search.SetPage(PageName.Interests);
        HttpContext.Session.Set("Search", Search);

        return Page();
    }

    public IActionResult OnPost() 
    {
        Search = HttpContext.Session.Get<Search>("Search") ?? new Search();

        if (string.IsNullOrEmpty(Search.Location) ||
            Search.Distance == null || Search.Distance == Distance.ThirtyPlus)
        {
            FirstOptionMandatory = true;
            
            if (string.IsNullOrWhiteSpace(UserInterest1))
            {
                ModelState.AddModelError("UserInterest1", "Please enter an interest");
            }
        }

        if (!ModelState.IsValid)
            return Page(); 
        
        Search.Interests.Clear();
        
        List<string> interests = new List<string>();
        if (!string.IsNullOrWhiteSpace(UserInterest1))
            interests.Add(UserInterest1.Trim());
        if (!string.IsNullOrWhiteSpace(UserInterest2))
            interests.Add(UserInterest2.Trim());
        if (!string.IsNullOrWhiteSpace(UserInterest3))
            interests.Add(UserInterest3.Trim());
        Search.Interests = interests;

        Search.Updated = true;
        HttpContext.Session.Set("Search", Search);
        
        return RedirectToPage(PageName.QualificationLevel); 
    }
}