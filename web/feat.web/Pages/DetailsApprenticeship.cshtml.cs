using feat.web.Extensions;
using feat.web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using feat.web.Utils;

namespace feat.web.Pages;

public class DetailsApprenticeshipModel(ILogger<DetailsApprenticeshipModel> logger) : PageModel
{
    [BindProperty] 
    public required string Id { get; set; }
    
    
    public required Search Search { get; set; }
    
    public void OnGet(string? id)
    {
        logger.LogInformation("OnGet called");

        try
        {
            Search = HttpContext.Session.Get<Search>("Search") ?? new Search();
            Search.SetPage(PageName.DetailsApprenticeship);
            
            Id = !string.IsNullOrEmpty(id) ? id : "missing id";
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
}