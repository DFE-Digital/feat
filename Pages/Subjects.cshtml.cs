using System.ComponentModel.DataAnnotations;
using feat.web.Extensions;
using feat.web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace feat.web.Pages;

public class SubjectsModel(ILogger<SubjectsModel> logger) : PageModel
{

    public void OnGet()
    {
        Search = HttpContext.Session.Get<Search>("Search") ?? new Search();
        if (Search.Subjects.HasValue)
            Subjects = Search.Subjects;
    }
    
    [BindProperty]
    [Required(ErrorMessage = "Please select if there are any subjects you're good at or enjoy, or click \"Skip this step\"")]
    public bool? Subjects { get; set; }
    
    public required Search Search { get; set; }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
            return Page();
        
        Search = HttpContext.Session.Get<Search>("Search") ?? new Search();
        
        if (Subjects.HasValue) Search.Subjects = Subjects.Value;

        HttpContext.Session.Set("Search", Search);

        return Subjects != null && Subjects.Value ? RedirectToPage("SubjectList") : RedirectToPage("JobsOrCareers");
    }
}