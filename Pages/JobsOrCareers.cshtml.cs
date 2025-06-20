using System.ComponentModel.DataAnnotations;
using feat.web.Extensions;
using feat.web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace feat.web.Pages;

public class JobsOrCareersModel(ILogger<JobsOrCareersModel> logger) : PageModel
{

    public void OnGet()
    {
        Search = HttpContext.Session.Get<Search>("Search") ?? new Search();
        if (Search.JobOrCareers.HasValue)
            JobOrCareers = Search.JobOrCareers;
    }
    
    [BindProperty]
    [Required(ErrorMessage = "Please select if there are any jobs or careers you're interested in, or click \"Skip this step\"")]
    public bool? JobOrCareers { get; set; }
    
    public required Search Search { get; set; }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
            return Page();
        
        Search = HttpContext.Session.Get<Search>("Search") ?? new Search();
        
        if (JobOrCareers.HasValue) Search.JobOrCareers = JobOrCareers.Value;

        HttpContext.Session.Set("Search", Search);

        return JobOrCareers != null && JobOrCareers.Value ? RedirectToPage("JobsOrCareersList") : RedirectToPage("Results");
    }
}