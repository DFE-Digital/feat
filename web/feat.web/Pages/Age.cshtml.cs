using System.ComponentModel.DataAnnotations;
using feat.web.Enums;
using feat.web.Extensions;
using feat.web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace feat.web.Pages;

public class AgeModel (ILogger<AgeModel> logger) : PageModel
{
    [BindProperty]
    [Required(ErrorMessage = "Please select your age group or select \"Skip this step\"")]
    public AgeGroup? AgeGroup { get; set; }
    
    public required Search Search { get; set; }
    
    public IActionResult OnGet()
    {
        Search = HttpContext.Session.Get<Search>("Search") ?? new Search();
        
        if (Search.AgeGroup.HasValue)
            AgeGroup = Search.AgeGroup;

        Search.SetPage(PageName.Age);//"Age");
        HttpContext.Session.Set("Search", Search);
        
        return Page();
    }

    public IActionResult OnPost()
    {
        logger.LogInformation("Age OnPost {AgeGroup}", AgeGroup);
        try
        {

        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            Console.WriteLine(e);
            throw;
        }
        Search = HttpContext.Session.Get<Search>("Search") ?? new Search();
        if (!ModelState.IsValid)
            return Page();
        
        //Search.SetPage("Age"); --?? 
        Search.Updated = true;
        
        if (AgeGroup.HasValue) 
            Search.AgeGroup = AgeGroup.Value;
        
        HttpContext.Session.Set("Search", Search);

        /*
        if (AgeGroup is Enums.AgeGroup.UnderEighteen or Enums.AgeGroup.EighteenToTwentyFour 
            && SearchType == Enums.SearchType.HE)
        {
            return RedirectToPage("Interests");
        }

        if (AgeGroup == Enums.AgeGroup.UnderEighteen 
            && SearchType is Enums.SearchType.FE or Enums.SearchType.Return)
        {
            return RedirectToPage("Location");
        } check-answers
        */

        return RedirectToPage(PageName.CheckAnswers);// need to go to CheckAnswer => Summary.
    }
}