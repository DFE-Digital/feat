using System.ComponentModel.DataAnnotations;
using feat.web.Enums;
using feat.web.Extensions;
using feat.web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace feat.web.Pages;

public class QualificationLevelModel (ILogger<QualificationLevelModel> logger) : PageModel
{
    [BindProperty]
    [Required(ErrorMessage = "Please select the level of qualification you are looking for")]
    public QualificationLevel? QualificationLevel { get; set; }
    
    public required Search Search { get; set; }
    
    public IActionResult OnGet()
    {
        Search = HttpContext.Session.Get<Search>("Search") ?? new Search();
        
        if (Search.QualificationLevel.HasValue)
            QualificationLevel = Search.QualificationLevel;

        Search.SetPage("QualificationLevel");
        HttpContext.Session.Set("Search", Search);
        
        return Page();
    }

    public IActionResult OnPost()
    {
        Search = HttpContext.Session.Get<Search>("Search") ?? new Search();
        if (!ModelState.IsValid)
            return Page();

        Search.Updated = true;
        if (QualificationLevel.HasValue) Search.QualificationLevel = QualificationLevel.Value;

        HttpContext.Session.Set("Search", Search);

        return RedirectToPage("Summary");

    }
}