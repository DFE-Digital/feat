using System.ComponentModel.DataAnnotations;
using feat.web.Enums;
using feat.web.Extensions;
using feat.web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace feat.web.Pages;

public class CourseTypeModel (ILogger<CourseTypeModel> logger) : PageModel
{
    [BindProperty]
    [Required(ErrorMessage = "Please select how you would like to search")]
    public CourseType? CourseType { get; set; }
    
    public required Search Search { get; set; }
    
    public IActionResult OnGet()
    {
        Search = HttpContext.Session.Get<Search>("Search") ?? new Search();
        
        if (Search.CourseType.HasValue)
            CourseType = Search.CourseType;

        Search.SetPage("CourseType");
        HttpContext.Session.Set("Search", Search);
        
        return Page();
    }

    public IActionResult OnPost()
    {
        Search = HttpContext.Session.Get<Search>("Search") ?? new Search();
        if (!ModelState.IsValid)
            return Page();

        Search.Updated = true;
        if (CourseType.HasValue) Search.CourseType = CourseType.Value;

        HttpContext.Session.Set("Search", Search);

        return RedirectToPage("CourseLevel");

    }
}