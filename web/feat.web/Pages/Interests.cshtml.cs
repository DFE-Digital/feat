using System.ComponentModel.DataAnnotations;
using feat.web.Enums;
using feat.web.Extensions;
using feat.web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace feat.web.Pages;

public class InterestsModel(ILogger<InterestsModel> logger) : PageModel
{
    [BindProperty]
    public string? Interest { get; set; }
    
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
        
        
        if (!string.IsNullOrEmpty(Interest) && !Search.Interests.Contains(Interest))
        {
                Search.Interests.Add(Interest);
        }

        if (Search.Interests.Count == 0)
        {
            ModelState.AddModelError("Interest", "Please enter at least one interest");
            return Page();       
        }
        
        Search.Updated = true;
        HttpContext.Session.Set("Search", Search);

        if (Search.SearchMethod == SearchMethod.Guided)
        {
            return RedirectToPage("QualificationLevel");
        }

        return RedirectToPage("Location");
    }

    public IActionResult OnPostRemove(int index)
    {
        Search = HttpContext.Session.Get<Search>("Search") ?? new Search();
        try
        {
            Search.Interests.RemoveAt(index);
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

        if (string.IsNullOrEmpty(Interest))
        {
            ModelState.AddModelError("Interest", "Please enter an interest");
            return Page();
        }

        if (!Search.Interests.Contains(Interest))
            Search.Interests.Add(Interest);

        HttpContext.Session.Set("Search", Search);
        
        ModelState.Clear();
        Interest = string.Empty;
        
        return Page();
    }
}