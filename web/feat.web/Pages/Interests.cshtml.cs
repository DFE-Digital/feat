using System.ComponentModel.DataAnnotations;
using feat.web.Enums;
using feat.web.Extensions;
using feat.web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace feat.web.Pages;

public class InterestsModel(ILogger<InterestsModel> logger) : PageModel
{
    //[BindProperty] public int NumberofOptions { get; set; } = 3;

    [BindProperty] 
    public List<string> UserInterests { get; set; } = new List<string>();
    
    [BindProperty] 
    public bool FirstOptionMandatory { get; set; } = false;
    
    public required Search Search { get; set; }
    
    public IActionResult OnGet()
    {
        Search = HttpContext.Session.Get<Search>("Search") ?? new Search();
        
        if (!Search.Updated)
            return RedirectToPage("Index");
        
        UserInterests = Search.Interests;
        
        // If selected distance > 30 miles, 
        if (string.IsNullOrEmpty(Search.Location) || Search.Distance == null || Search.Distance == Distance.ThirtyPlus)
        {
            FirstOptionMandatory = true;
        }

        Search.SetPage("Interests");
        HttpContext.Session.Set("Search", Search);
        
        return Page();
    }
    
    public IActionResult OnPostContinue()
    {
        Search = HttpContext.Session.Get<Search>("Search") ?? new Search();

        foreach (var interest in UserInterests)
        {
            if (!string.IsNullOrEmpty(interest) && !Search.Interests.Contains(interest))
            {
                Search.Interests.Add(interest);
            }
        }
/*
        if (!string.IsNullOrEmpty(Interest) && !Search.Interests.Contains(Interest))
        {
            Search.Interests.Add(Interest);
        }*/

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

        return RedirectToPage("Location");// Where to Go Next? ... Defo Not location
    }

    public IActionResult OnPostRemove(int index)
    {
        Search = HttpContext.Session.Get<Search>("Search") ?? new Search();
        try
        {
            Search.Interests.RemoveAt(index);
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

        if (!UserInterests.Any()) // .IsNullOrEmpty(Interest))
        {
            ModelState.AddModelError("Interest", "Please enter an interest");
            return Page();
        }

        //if (!Search.Interests.Contains(Interest))
        //    Search.Interests.Add(Interest);
        foreach (var interest in UserInterests)
        {
            if (!string.IsNullOrEmpty(interest) && !Search.Interests.Contains(interest))
            {
                Search.Interests.Add(interest);
            }
        }
        
        HttpContext.Session.Set("Search", Search);
        
        ModelState.Clear();
        //Interest = string.Empty;
        UserInterests = new List<string>();
        
        return Page();
    }

}