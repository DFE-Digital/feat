using System.ComponentModel.DataAnnotations;
using feat.web.Extensions;
using feat.web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace feat.web.Pages;

public class CareersModel(ILogger<CareersModel> logger) : PageModel
{
    [BindProperty]
    public string? Career { get; set; }
    
    public required Search Search { get; set; }
    
    public IActionResult OnGet()
    {
        Search = HttpContext.Session.Get<Search>("Search") ?? new Search();
        
        if (!Search.Updated)
            return RedirectToPage("Index");
        
        Search.SetPage("Interests");
        HttpContext.Session.Set("Search", Search);
        
        return Page();
    }
    
    public IActionResult OnPostContinue()
    {
        Search = HttpContext.Session.Get<Search>("Search") ?? new Search();
        
        
        if (!string.IsNullOrEmpty(Career) && !Search.Careers.Contains(Career))
        {
                Search.Careers.Add(Career);
        }

        if (Search.Careers.Count == 0)
        {
            ModelState.AddModelError("Career", "Please enter at least one job or career, or click \"Skip this step\"");
            return Page();       
        }
        
        Search.Updated = true;
        HttpContext.Session.Set("Search", Search);

        return RedirectToPage("CourseType");
    }

    public IActionResult OnPostRemove(int index)
    {
        Search = HttpContext.Session.Get<Search>("Search") ?? new Search();
        try
        {
            Search.Careers.RemoveAt(index);
            HttpContext.Session.Set("Search", Search);
        }
        catch (Exception)
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

        if (string.IsNullOrEmpty(Career))
        {
            ModelState.AddModelError("Career", "Please enter a career or click \"Skip this step\"");
            return Page();
        }

        if (!Search.Careers.Contains(Career))
            Search.Careers.Add(Career);

        HttpContext.Session.Set("Search", Search);
        
        ModelState.Clear();
        Career = string.Empty;
        
        return Page();
    }
}