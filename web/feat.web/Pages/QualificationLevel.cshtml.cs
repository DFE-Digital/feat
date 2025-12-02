using System.ComponentModel.DataAnnotations;
using feat.web.Enums;
using feat.web.Extensions;
using feat.web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using feat.web.Utils;

namespace feat.web.Pages;

public class QualificationLevelModel (ILogger<QualificationLevelModel> logger) : PageModel
{
    [BindProperty]
    [Required(ErrorMessage = "Please select the level of qualification you are looking for")]
    public List<QualificationLevel> SelectedQualificationOptions { get; set; } = [];
    
    public required Search Search { get; set; }
    
    public IActionResult OnGet()
    {
        Search = HttpContext.Session.Get<Search>("Search") ?? new Search();
        
        if (Search.QualificationLevels.Count != 0)
            SelectedQualificationOptions = Search.QualificationLevels;
        
        Search.SetPage(PageName.QualificationLevel); 
        HttpContext.Session.Set("Search", Search);
        
        return Page();
    }

    public IActionResult OnPost()
    {
        logger.LogInformation("Qualification OnPost {SelectedQualificationOptions}", SelectedQualificationOptions);

        try
        {
            Search = HttpContext.Session.Get<Search>("Search") ?? new Search();
            
            if (SelectedQualificationOptions?.Count == 0)
            {
                ModelState.AddModelError("SelectedQualificationOptions", SharedStrings.SelectQualificationLevel);
            }
            
            if (!ModelState.IsValid) 
                return Page();
            
            Search.QualificationLevels?.Clear();
            if (SelectedQualificationOptions is { Count: > 0 })
            {
                foreach (var qualificationOption in SelectedQualificationOptions)
                {
                    if (Search.QualificationLevels != null && 
                        !Search.QualificationLevels.Contains(qualificationOption))
                    {
                        Search.QualificationLevels.Add(qualificationOption);
                    }
                }
            }
            else
            {
                Search.QualificationLevels = new List<QualificationLevel>(Enum.GetValues<QualificationLevel>());
            }
            Search.Updated = true;

            HttpContext.Session.Set("Search", Search);

            if ((SelectedQualificationOptions.Contains(Enums.QualificationLevel.None) || 
                 SelectedQualificationOptions.Contains(Enums.QualificationLevel.OneAndTwo)))
            {
                if (Search.AgeGroup.HasValue && Search.VisitedCheckAnswers)
                {
                    return RedirectToPage(PageName.CheckAnswers);
                }
                return RedirectToPage(PageName.Age);
            }
            
            return RedirectToPage(PageName.CheckAnswers);
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
        }
        return Page();
    }
    
}