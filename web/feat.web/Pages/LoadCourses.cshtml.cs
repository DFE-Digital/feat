using feat.common.Models;
using feat.web.Enums;
using feat.web.Extensions;
using feat.web.Models;
using feat.web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NetTopologySuite.Operation.Valid;
using feat.common.Models.Enums;

namespace feat.web.Pages;

public class LoadCoursesModel(ISearchService searchService, ILogger<LoadCoursesModel> logger) : PageModel
{
    [BindProperty]
    public List<AttendancePattern> SelectedCourseHours { get; set; } = new();
    
    public List<CourseType> SelectedCourseTypes { get; set; } = new();
    public List<QualificationLevel> SelectedQualificationLevels { get; set; } = new();
    
    [BindProperty]
    public Distance? SelectedTravelDistance { get; set; } = new();
    
    
    public required Search Search { get; set; }
    
    public SearchResponse? SearchResponse { get; set; }
    
    [BindProperty]
    public Pagination PaginationState { get; set; } = new();
    
    [BindProperty]
    public string SortBy { get; set; } = "Distance";
    
    public async Task<IActionResult> OnGetAsync([FromQuery] bool debug = false)
    {
        logger.LogInformation("OnGetAsync called");
        try
        {
            Search = HttpContext.Session.Get<Search>("Search") ?? new Search();
            if (!Search.Updated)
            {
                return RedirectToPage(PageName.Index);
            }
            
            Search.Debug = debug;
            Search.SetPage(PageName.LoadCourses);
            HttpContext.Session.Set("Search", Search);
            
            
            SearchResponse = await searchService.Search(Search, HttpContext.Session.Id);
            
            // Set up data 
            List<Facet> allFacets = SearchResponse?.Facets.ToList() ?? new List<Facet>();
            
            // set distance - as it was chosen by the user previously
            SelectedTravelDistance = Search.Distance;
            
            
            PaginationState = new Pagination()
            {
                PageNumber = SearchResponse.Page,
                PageSize = SearchResponse.PageSize,
                TotalPageCount = SearchResponse.TotalCount.HasValue ? SearchResponse.TotalCount.Value : 0,
            };

            SortBy = SearchResponse.SortBy; 
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
        }
        return Page();
    }

    public async Task<IActionResult> OnPostSort(string sortBy)
    {
        logger.LogInformation("OnPostSort called ");
        
        if (!string.IsNullOrEmpty(sortBy))
        {
            if (sortBy == "relevance" || sortBy == "distance")
            {
                logger.LogInformation("OnPostSort called {sortBy}", sortBy);
            }
            
            SearchResponse = await searchService.GetSortedCourses(sortBy);    
        }
        return Page();
    }

    public IActionResult OnPostClearFilter()
    {
        logger.LogDebug("OnPostClearFilter called");
        
        // Clear the bound properties
        ModelState.Clear();
        return Page();
    }

    public async Task<IActionResult> OnPost()
    {
        await Task.Delay(1);
        return Page();
    }

}

public class Pagination
{
    public int PageNumber { get; set; } = 1;
    public int PageSize  { get; set; } = 10;
    public long TotalPageCount  { get; set; } = 15;
}