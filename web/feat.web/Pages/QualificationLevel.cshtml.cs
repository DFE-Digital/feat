using System.Text.Json;
using feat.web.Enums;
using feat.web.Extensions;
using feat.web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using feat.web.Utils;

namespace feat.web.Pages;

public class QualificationLevelModel (ILogger<QualificationLevelModel> logger) : PageModel
{
    public required Search Search { get; set; }
    
    [BindProperty]
    public List<QualificationLevel> SelectedQualificationOptions { get; set; } = [];
    
    [TempData]
    public string? SelectedQualificationsJson { get; set; }
    
    [TempData]
    public string? ValidationError { get; set; }
    
    public IActionResult OnGet()
    {
        Search = HttpContext.Session.Get<Search>("Search") ?? new Search();

        if (!Search.Updated)
        {
            return RedirectToPage("Index");
        }

        if (!string.IsNullOrEmpty(SelectedQualificationsJson))
        {
            SelectedQualificationOptions =
                JsonSerializer.Deserialize<List<QualificationLevel>>(SelectedQualificationsJson) ?? [];
        }
        else if (Search.QualificationLevels.Count != 0)
        {
            SelectedQualificationOptions = Search.QualificationLevels;
        }

        if (!string.IsNullOrEmpty(ValidationError))
        {
            ModelState.AddModelError(nameof(SelectedQualificationOptions), ValidationError);
        }

        Search.SetPage(PageName.QualificationLevel);
        HttpContext.Session.Set("Search", Search);

        return Page();
    }

    public IActionResult OnPost()
    {
        logger.LogInformation("Selected Qualification Levels {@Levels}", SelectedQualificationOptions);

        Search = HttpContext.Session.Get<Search>("Search") ?? new Search();

        if (SelectedQualificationOptions.Count == 0)
        {
            ValidationError = SharedStrings.SelectQualificationLevel;
            SelectedQualificationsJson = JsonSerializer.Serialize(SelectedQualificationOptions);

            return RedirectToPage();
        }

        Search.QualificationLevels.Clear();
        
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

        if (SelectedQualificationOptions.Contains(QualificationLevel.None) ||
             SelectedQualificationOptions.Contains(QualificationLevel.OneAndTwo))
        {
            return RedirectToPage(Search is { AgeGroup: not null, VisitedCheckAnswers: true }
                ? PageName.CheckAnswers
                : PageName.Age);
        }

        if (Search is { AgeGroup: not null, VisitedCheckAnswers: true })
        {
            Search.AgeGroup = null;
            HttpContext.Session.Set("Search", Search);
        }

        return RedirectToPage(PageName.CheckAnswers);
    }
}