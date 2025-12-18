using feat.web.Enums;
using feat.web.Extensions;
using feat.web.Models;
using feat.web.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace feat.web.Pages;

public class AgeModel(ILogger<AgeModel> logger) : PageModel
{
    [BindProperty]
    public AgeGroup? AgeGroup { get; set; }

    public required Search Search { get; set; }

    public IActionResult OnGet()
    {
        logger.LogInformation("OnGet called");
        
        Search = HttpContext.Session.Get<Search>("Search") ?? new Search();

        if (!Search.Updated)
        {
            return RedirectToPage("Index");
        }
        
        if (Search.AgeGroup.HasValue)
            AgeGroup = Search.AgeGroup;

        Search.SetPage(PageName.Age);
        HttpContext.Session.Set("Search", Search);

        return Page();
    }

    public IActionResult OnPost()
    {
        logger.LogInformation("OnPost called");

        Search = HttpContext.Session.Get<Search>("Search") ?? new Search();
        if (!ModelState.IsValid)
            return Page();

        if (AgeGroup.HasValue)
            Search.AgeGroup = AgeGroup.Value;

        Search.Updated = true;
        HttpContext.Session.Set("Search", Search);

        return RedirectToPage(PageName.CheckAnswers);
    }
}