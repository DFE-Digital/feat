using System.ComponentModel.DataAnnotations;
using feat.web.Extensions;
using feat.web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace feat.web.Pages;

public class SubjectListModel(ILogger<SubjectListModel> logger) : PageModel
{

    public void OnGet()
    {
        Search = HttpContext.Session.Get<Search>("Search") ?? new Search();
    }
    
    [BindProperty]
    public string? Subject { get; set; }
    
    public required Search Search { get; set; }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
            return Page();
        
        Search = HttpContext.Session.Get<Search>("Search") ?? new Search();
        
        

        HttpContext.Session.Set("Search", Search);

        return RedirectToPage("JobsOrCareers");
    }
}