using feat.web.Extensions;
using feat.web.Models;
using feat.web.Repositories;
using feat.web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SessionExtensions = Microsoft.AspNetCore.Http.SessionExtensions;

namespace feat.web.Pages;

public class ResultsModel(ISearchService searchService) : PageModel
{
    public required Search Search { get; set; }
    
    public FindAResponse? FindAResponse { get; set; }
    
    
    public async Task<IActionResult> OnGetAsync([FromQuery] bool debug = false)
    {
        Search = HttpContext.Session.Get<Search>("Search") ?? new Search();
        if (!Search.Updated)
            return RedirectToPage("Index");

        Search.Debug = debug;
        Search.SetPage("Results");
        HttpContext.Session.Set("Search", Search);

        FindAResponse = await searchService.Search(Search, HttpContext.Session.Id);
        
        return Page();
    }
}