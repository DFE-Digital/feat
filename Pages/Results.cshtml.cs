using feat.web.Extensions;
using feat.web.Models;
using feat.web.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SessionExtensions = Microsoft.AspNetCore.Http.SessionExtensions;

namespace feat.web.Pages;

public class ResultsModel(HttpClientRepository httpClientRepository) : PageModel
{
    public required Search Search { get; set; }
    
    public FindAResponse? FindAResponse { get; set; }
    
    [BindProperty]
    public FindARequest? FindARequest { get; set; }
    
    public async Task<IActionResult> OnGetAsync()
    {
        Search = HttpContext.Session.Get<Search>("Search") ?? new Search();
        if (!Search.Updated)
            return RedirectToPage("Index");

        Search.SetPage("Results");
        HttpContext.Session.Set("Search", Search);
        
        FindARequest = Search.ToFindARequest();
        FindARequest.SessionId = HttpContext.Session.Id;

        string url = "https://find-a-dev-def2ardzbwgvfzfm.westeurope-01.azurewebsites.net/api/Search";
        // string url = "https://localhost:7121/api/Search";
        
        FindAResponse = await httpClientRepository.PostAsync<FindAResponse>(url, FindARequest);
        
        return Page();
    }
}