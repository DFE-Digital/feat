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
        logger.LogInformation("OnGet called");
        
        Search = HttpContext.Session.Get<Search>("Search") ?? new Search();
        if (!Search.Updated)
            return RedirectToPage("Index");

        Search.SetPage(PageName.CheckAnswers);
        Search.ResetHistory(PageName.CheckAnswers);
        HttpContext.Session.Set("Search", Search);
        
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