using feat.web.Extensions;
using feat.web.Models;
using feat.web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace feat.web.Pages;

public class ResultsModel(ISearchService searchService) : PageModel
{
    public required Search Search { get; set; }
    
    public SearchResponse? SearchResponse { get; set; }
    
    public async Task<IActionResult> OnGetAsync([FromQuery] bool debug = false)
    {
        Search = HttpContext.Session.Get<Search>("Search") ?? new Search();
        if (!Search.Updated)
        {
            return RedirectToPage("Index");
        }

        Search.Debug = debug;
        Search.SetPage("Results");
        HttpContext.Session.Set("Search", Search);

        SearchResponse = await searchService.Search(Search, HttpContext.Session.Id);
        
        return Page();
    }
}