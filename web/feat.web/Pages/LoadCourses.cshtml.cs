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
    
    public List<QualificationLevel> SelectedCourseTypes { get; set; } = new();
    public List<QualificationLevel> SelectedQualificationLevels { get; set; } = new();
    public List<QualificationLevel> SelectedLearningMethods { get; set; } = new();
    public List<QualificationLevel> SelectedCourseStudyTimes { get; set; } = new();
    
    
    [BindProperty]
    public Distance? SelectedTravelDistance { get; set; } = new();
    
    
    
    //--
    public required Search Search { get; set; }
    
    public SearchResponse? SearchResponse { get; set; }
    
    public int PageNumber
    {
        get
        {
            return SearchResponse == null ? 1 : SearchResponse.Page;
        }
    }
    
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
            
            //TODO- Hack a fixed search; REMOVE 
            Search = new Search()
            {
                Debug = debug, 
                Interests = new List<string>() { "biotech"}, 
                Location = "London",
                AgeGroup = AgeGroup.Eighteen,
                Distance = Distance.ThirtyPlus,
                QualificationLevels = new List<QualificationLevel>(){ QualificationLevel.None, QualificationLevel.OneAndTwo},
                IncludeOnlineCourses = true, 
                Updated = true,
                //History = just for navigation,
            }; 

            SearchResponse = await searchService.Search(Search, HttpContext.Session.Id);
            
            // Set up data 
            
            List<Facet> allFacets = SearchResponse?.Facets.ToList() ?? new List<Facet>();
            
            // set distance - as it was chosen by the user previously
            SelectedTravelDistance = Search.Distance;
            
            // Pagination
            var pageNumber = SearchResponse?.Page;
            var pageSize = SearchResponse?.PageSize;
            var totalPageCount = SearchResponse?.TotalCount;
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
        }
        return Page();
    }

    public IActionResult OnPostClearFilter(string filterNumber = "")
    {
        logger.LogDebug("OnPostClearFilter called; {filterNumber}", filterNumber);
        if (!string.IsNullOrEmpty(filterNumber))
        {
            //SelectedCourseHours.Clear();

            // attendancePattern[@i]
            ModelState.Clear();
            
            return Page();
        }

        //await Task.Delay(1);

        return Page(); //RedirectToPage(PageName.Index);
    }

    public async Task<IActionResult> OnPost()
    {
        //return RedirectToPage(PageName.Interests);
        await Task.Delay(1);
        return Page();
    }

    public IActionResult OnPostGoToDetailsScreen([FromQuery] string courseId)
    {
        logger.LogDebug("OnPostGoToDetailsScreen called; {courseId}", courseId);
        logger.LogInformation("OnPostGoToDetailsScreen called");
        
        if(string.IsNullOrEmpty(courseId))
            return Page();
        
        return RedirectToPage(PageName.Interests);
    }
}

internal class Pagination
{
    
}