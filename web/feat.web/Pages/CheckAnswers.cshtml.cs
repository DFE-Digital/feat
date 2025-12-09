using feat.web.Enums;
using feat.web.Extensions;
using feat.web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using feat.web.Utils;

namespace feat.web.Pages;

public class CheckAnswersModel (ILogger<CheckAnswersModel> logger): PageModel
{
    public required Search Search { get; set; }
    
    public IActionResult OnGet()
    {
        Search = HttpContext.Session.Get<Search>("Search") ?? new Search();
        
        if (!Search.Updated)
        {
            return RedirectToPage("Index");
        }

        HttpContext.Session.Remove("AllFacets");
        Search.Facets.Clear();
        
        Search.SetPage(PageName.CheckAnswers);
        Search.ResetHistory(PageName.CheckAnswers);
        
        HttpContext.Session.Set("Search", Search);

        if (Search.Interests.Count == 0 && (Search.Distance is null or < Distance.Two) && 
            Search.QualificationLevels.Count == 0 && Search.VisitedCheckAnswers)
        {
            return RedirectToPage(PageName.Location);
        }
        
        return Page();
    }

    public IActionResult OnPost()
    {
        logger.LogInformation("OnPost called");
        try
        {
            Search = HttpContext.Session.Get<Search>("Search") ?? new Search();
            
            Search.Updated = true;
            HttpContext.Session.Set("Search", Search);

            return RedirectToPage(PageName.LoadCourses);
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            throw;
        }
    }
}