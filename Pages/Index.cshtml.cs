using System.ComponentModel.DataAnnotations;
using feat.web.Enums;
using feat.web.Extensions;
using feat.web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace feat.web.Pages;

public class IndexModel(ILogger<IndexModel> logger) : PageModel
{
    public void OnGet()
    {
        Search = HttpContext.Session.Get<Search>("Search") ?? new Search();
        if (Search.AgeGroup.HasValue)
            AgeGroup = Search.AgeGroup;
    }
    
    [BindProperty]
    [Required(ErrorMessage = "Please select an age group, or click \"Skip this step\"")]
    public AgeGroup? AgeGroup { get; set; }
    
    public required Search Search { get; set; }

    public IActionResult OnPost()
    {
        Search = HttpContext.Session.Get<Search>("Search") ?? new Search();
        
        if (!ModelState.IsValid)
            return Page();
        
        Search.AgeGroup = AgeGroup;
        HttpContext.Session.Set("Search", Search);

        return RedirectToPage("Location");
    }
}