using System.ComponentModel.DataAnnotations;
using feat.web.Enums;
using feat.web.Extensions;
using feat.web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace feat.web.Pages;

public class HowModel (ILogger<HowModel> logger) : PageModel
{
    [BindProperty]
    [Required(ErrorMessage = "Please select how you would like to search")]
    public SearchMethod? SearchMethod { get; set; }
    
    public required Search Search { get; set; }
    
    public IActionResult OnGet()
    {
        Search = HttpContext.Session.Get<Search>("Search") ?? new Search();
        
        if (Search.SearchMethod.HasValue)
            SearchMethod = Search.SearchMethod;

        Search.SetPage("How");
        HttpContext.Session.Set("Search", Search);
        
        return Page();
    }

    public IActionResult OnPost()
    {
        Search = HttpContext.Session.Get<Search>("Search") ?? new Search();
        if (!ModelState.IsValid)
            return Page();

        Search.Updated = true;
        if (SearchMethod.HasValue) Search.SearchMethod = SearchMethod.Value;

        HttpContext.Session.Set("Search", Search);

        if (SearchMethod is Enums.SearchMethod.ByName)
        {
            return RedirectToPage("Interests");
        }
        
        if (SearchMethod is Enums.SearchMethod.Guided && Search.AgeGroup is AgeGroup.UnderEighteen)
        {
            return RedirectToPage("Subjects");
        }

        return RedirectToPage("Location");

    }
}