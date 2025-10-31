using feat.web.Enums;
using feat.web.Extensions;
using feat.web.Models;
using feat.web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using feat.common.Models.Enums;
using feat.web.Utils;

namespace feat.web.Pages;

public class LoadCoursesModel(ISearchService searchService, ILogger<LoadCoursesModel> logger) : PageModel
{
    [BindProperty]
    public List<CourseHours> SelectedCourseHours { get; set; } = new();
    
    public List<Enums.CourseType> SelectedCourseTypes { get; set; } = new();
    public List<Enums.QualificationLevel> SelectedQualificationLevels { get; set; } = new();
    
    [BindProperty]
    public Distance? SelectedTravelDistance { get; set; } = new();
    
    
    public required Search Search { get; set; }
    
    
    public List<Course> Courses { get; set; } = [];
    
    public int TotalCourseCount { get; private set; }


    // Pagination logic
    public int CurrentPage { get; set; } = 1;
    public int TotalPages { get; set; }
    public int PageSize { get; set; } = 3;
    
    // Pagination helpers
    public bool HasPreviousPage => CurrentPage > 1;
    public bool HasNextPage => CurrentPage < TotalPages;
    public int PreviousPage => CurrentPage - 1;
    public int NextPage => CurrentPage + 1;
    
    public async Task<IActionResult> OnGetAsync(string orderBy, int pageNumber = 1, [FromQuery] bool debug = false) 
    {
        logger.LogInformation("OnGetAsync called");
        try
        {
            Search = HttpContext.Session.Get<Search>("Search") ?? new Search();
            if (!Search.Updated)
            {
                return RedirectToPage(PageName.Index);
            }
            
            // pagination
            if(pageNumber > 0)
                Search.CurrentPage = pageNumber;
            Search.TotalPages = TotalPages;
            Search.PageSize = PageSize;
            
            if (!string.IsNullOrEmpty(orderBy))
            {
                Search.OrderBy = orderBy.Equals("distance", StringComparison.InvariantCultureIgnoreCase) ? OrderBy.Distance : OrderBy.Relevance;
            }
            
            Search.Debug = debug;
            Search.SetPage(PageName.LoadCourses);
            HttpContext.Session.Set("Search", Search);
            
            var searchResponse = await searchService.Search(Search, HttpContext.Session.Id);
            
            if (searchResponse.SearchResults.Count == 0)
            {
                return RedirectToPage(PageName.NoResultsSearch);
            }

            // Set up data :
            TotalCourseCount = (int)searchResponse.TotalCount;
            Courses = searchResponse.SearchResults.ToCourses();
            
            // Filter & Facets
            
            // set distance - as it was chosen by the user previously
            SelectedTravelDistance = Search.Distance;
            
            CurrentPage = searchResponse.Page;
            PageSize = searchResponse.PageSize;
            TotalPages = (int)Math.Ceiling(searchResponse.TotalCount / (double)searchResponse.PageSize);
            
            // Save back into Session
            
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
        }
        return Page();
    }

    public IActionResult OnPostClearFilters()
    {
        logger.LogDebug("OnPostClearFilter called");
        
        //TODO Clear the filter properties
        ModelState.Clear();
        return Page();
    }

    public async Task<IActionResult> OnPost()
    {
        await Task.Delay(1);
        return Page();
    }

    internal List<int> GetPageNumbers()
    {
        List<int> pages = new List<int>();

        if (TotalPages <= 3)
        {
            for (int i = 0; i < TotalPages; i++)
            {
                pages.Add(i);
            }
        }
        else
        {
            if (CurrentPage == 1)
            {
                pages.Add(1);
                pages.Add(2);
                pages.Add(3);
            }
            else if (CurrentPage == TotalPages)
            {
                // At the End - last 3pages
                pages.Add(TotalPages - 2);
                pages.Add(TotalPages - 1);
                pages.Add(TotalPages);
            }
            else
            {
                // In the middle
                pages.Add(CurrentPage - 1);
                pages.Add(CurrentPage);
                pages.Add(CurrentPage + 1);
            }
        }
        return pages;
    }

}
