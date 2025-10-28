using feat.web.Extensions;
using feat.web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace feat.web.Pages.CourseDetails;

public class DetailsCourseModel(ILogger<DetailsCourseModel> logger) : PageModel
{
    [BindProperty] 
     public string Id { get; set; }
     
    
    public required Search Search { get; set; }
    
    public void OnGet(string? id)
    {
        logger.LogInformation("OnGet called");
        try
        {
            Search = HttpContext.Session.Get<Search>("Search") ?? new Search();
            Search.SetPage(PageName.DetailsCourse);
            
            Id = !string.IsNullOrEmpty(id) ? id : "missing id";
            
            
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
 
    public IActionResult OnPost()
    {
        logger.LogInformation("OnPost called");

        return Page();
    }
}