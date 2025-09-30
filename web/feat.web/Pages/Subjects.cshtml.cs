using System.ComponentModel.DataAnnotations;
using feat.web.Extensions;
using feat.web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace feat.web.Pages;

public class SubjectsModel(ILogger<SubjectsModel> logger) : PageModel
{
    [BindProperty]
    public string? Subject { get; set; }
    
    public required Search Search { get; set; }
    
    public IActionResult OnGet()
    {
        Search = HttpContext.Session.Get<Search>("Search") ?? new Search();
        
        if (!Search.Updated)
            return RedirectToPage("Index");
        
        Search.SetPage("Subjects");
        HttpContext.Session.Set("Search", Search);
        
        return Page();
    }
    
    public IActionResult OnPostContinue()
    {
        Search = HttpContext.Session.Get<Search>("Search") ?? new Search();
        
        
        if (!string.IsNullOrEmpty(Subject) && !Search.Subjects.Contains(Subject))
        {
                Search.Subjects.Add(Subject);
        }

        if (Search.Subjects.Count == 0)
        {
            ModelState.AddModelError("Subject", "Please enter at least one subject or click \"Skip this step\"");
            return Page();       
        }
        
        Search.Updated = true;
        HttpContext.Session.Set("Search", Search);

        return RedirectToPage("Careers");
    }

    public IActionResult OnPostRemove(int index)
    {
        Search = HttpContext.Session.Get<Search>("Search") ?? new Search();
        try
        {
            Search.Subjects.RemoveAt(index);
            HttpContext.Session.Set("Search", Search);
        }
        catch (Exception ex)
        {
            // Do nothing
        }

        return Page();
    }

    public IActionResult OnPostAdd()
    {
        Search = HttpContext.Session.Get<Search>("Search") ?? new Search();
        
        if (!ModelState.IsValid)
            return Page();

        if (string.IsNullOrEmpty(Subject))
        {
            ModelState.AddModelError("Subject", "Please enter a subject or click \"Skip this step\"");
            return Page();
        }

        if (!Search.Subjects.Contains(Subject))
            Search.Subjects.Add(Subject);

        HttpContext.Session.Set("Search", Search);
        
        ModelState.Clear();
        Subject = string.Empty;
        
        return Page();
    }
}