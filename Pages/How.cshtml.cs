using System.ComponentModel.DataAnnotations;
using feat.web.Enums;
using feat.web.Extensions;
using feat.web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace feat.web.Pages;

public class HowModel (ILogger<HowModel> logger) : PageModel
{
    
    public void OnGet()
    {
        Search = HttpContext.Session.Get<Search>("Search") ?? new Search();
        if (Search.SearchMethod.HasValue)
            SearchMethod = Search.SearchMethod;
        
    }
    
    [BindProperty]
    [Required(ErrorMessage = "Please select how you would like to search, or click \"Skip this step\"")]
    public SearchMethod? SearchMethod { get; set; }
    
    public required Search Search { get; set; }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
            return Page();
        
        Search = HttpContext.Session.Get<Search>("Search") ?? new Search();
        if (SearchMethod.HasValue) Search.SearchMethod = SearchMethod.Value;
        
        HttpContext.Session.Set("Search", Search);
        return SearchMethod switch
        {
            Enums.SearchMethod.AllCoursesNearby => RedirectToPage("Results"),
            _ => RedirectToPage("Subjects")
        };
    }
}