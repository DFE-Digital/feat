using feat.web.Enums;
using feat.web.Extensions;
using feat.web.Models;
using feat.web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using feat.web.Utils;

namespace feat.web.Pages;

public class LoadCoursesModel(ISearchService searchService, ILogger<LoadCoursesModel> logger) : PageModel
{   
    [BindProperty]
    public Distance? SelectedTravelDistance { get; set; }
    
    public required Search Search { get; set; }
    
    public List<Course> Courses { get; set; } = [];
    
    public int TotalCourseCount { get; private set; }
    
    [BindProperty]
    public List<Models.ViewModels.Facet> AllFacets { get; set; } = [];
    
    // Pagination 
    public int CurrentPage { get; set; } = 1;
    public int TotalPages { get; set; }
    public int PageSize { get; set; } = 10;
    
    public bool HasPreviousPage => CurrentPage > 1;
    public bool HasNextPage => CurrentPage < TotalPages;
    public int PreviousPage => CurrentPage - 1;
    public int NextPage => CurrentPage + 1;
    
    public async Task<IActionResult> OnGetAsync(string orderBy, int pageNumber = 1) 
    {
        try
        {
            Search = HttpContext.Session.Get<Search>("Search") ?? new Search();
            
            if (!Search.Updated)
            {
                return RedirectToPage(PageName.Index);
            }

            if (pageNumber > 0)
            {
                Search.CurrentPage = pageNumber;
            }

            Search.TotalPages = TotalPages;
            Search.PageSize = PageSize;

            if (!string.IsNullOrEmpty(orderBy))
            {
                Search.OrderBy = orderBy.Equals("distance", StringComparison.InvariantCultureIgnoreCase) 
                    ? OrderBy.Distance 
                    : OrderBy.Relevance;
            }
            
            Search.SetPage(PageName.LoadCourses);
            HttpContext.Session.Set("Search", Search);
            
            var searchResponse = await searchService.Search(Search, HttpContext.Session.Id);
            if (!searchResponse.Courses.Any())
            {
                return RedirectToPage(PageName.NoResultsSearch);
            }

            TotalCourseCount = searchResponse.TotalCount;
            Courses = searchResponse.Courses.ToList();
            
            AllFacets = searchResponse.Facets.ToViewModels();
            
            if (Search.Facets.Count == 0) // Initial search
            {
                SelectQualificationLevels();

                Search.Facets = AllFacets;
            }
            
            var sessionFacets = HttpContext.Session.Get<List<Models.ViewModels.Facet>>("AllFacets");
            if (sessionFacets != null)
            {
                foreach (var facet in AllFacets)
                {
                    var sessionFacet = sessionFacets.FirstOrDefault(f => f.Name == facet.Name);
                    if (sessionFacet != null)
                    {
                        foreach (var value in facet.Values)
                        {
                            var sessionVal = sessionFacet.Values.FirstOrDefault(v => v.Name == value.Name);
                            
                            if (sessionVal != null)
                            {
                                value.Selected = sessionVal.Selected;
                            }
                        }
                    }
                }
            }
            
            HttpContext.Session.Set("AllFacets", AllFacets);
            Search.Facets = AllFacets;
            
            SelectedTravelDistance = Search.Distance;

            CurrentPage = searchResponse.Page;
            PageSize = searchResponse.PageSize;
            TotalPages = (int)Math.Ceiling(searchResponse.TotalCount / (double)searchResponse.PageSize);
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
        }

        return Page();
    }

    public IActionResult OnGetClearFilters()
    {
        Search = HttpContext.Session.Get<Search>("Search") ?? new Search();
        
        HttpContext.Session.Remove("AllFacets");
        AllFacets = [];
        
        Search.Facets = [];
        Search.CurrentPage = 1;
        
        Search.Distance = Search.OriginalDistance;
        Search.QualificationLevels = [];
        
        HttpContext.Session.Set("Search", Search);
        
        return RedirectToPage();
    }
    
    public IActionResult OnGetClearFacet(string facetName)
    {
        Search = HttpContext.Session.Get<Search>("Search") ?? new Search();

        var allFacets = HttpContext.Session.Get<List<Models.ViewModels.Facet>>("AllFacets") ?? [];
        
        var facet = allFacets.FirstOrDefault(f =>
            f.Name.Equals(facetName, StringComparison.InvariantCultureIgnoreCase));

        if (facet != null)
        {
            foreach (var value in facet.Values)
            {
                value.Selected = false;
            }
        }
        
        Search.Facets = allFacets;
        Search.CurrentPage = 1;

        HttpContext.Session.Set("AllFacets", allFacets);
        HttpContext.Session.Set("Search", Search);

        return RedirectToPage();
    }

    public IActionResult OnPostUpdateSelection()
    {
        logger.LogDebug("OnPostUpdateSelection called");
        
        Search = HttpContext.Session.Get<Search>("Search") ?? new Search();
        Search.Distance = SelectedTravelDistance;
        
        MergeSelectedFacets(AllFacets);
        
        HttpContext.Session.Set("AllFacets", AllFacets);
        HttpContext.Session.Set("Search", Search);

        return RedirectToPage();
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
    
    private void MergeSelectedFacets(List<Models.ViewModels.Facet>? postedFacets)
    {
        if (postedFacets == null || postedFacets.Count == 0)
        {
            return;
        }

        var fullFacets = HttpContext.Session.Get<List<Models.ViewModels.Facet>>("AllFacets") ?? [];

        if (fullFacets.Count == 0)
        {
            AllFacets = postedFacets;
            Search.Facets = postedFacets;
            
            return;
        }

        for (var pIndex = 0; pIndex < postedFacets.Count; pIndex++)
        {
            var posted = postedFacets[pIndex];

            Models.ViewModels.Facet? original = null;

            if (!string.IsNullOrEmpty(posted.Name))
            {
                original = fullFacets.FirstOrDefault(f =>
                    string.Equals(f.Name, posted.Name, StringComparison.InvariantCultureIgnoreCase));
            }

            if (original == null && pIndex < fullFacets.Count)
            {
                original = fullFacets[pIndex];
            }

            if (original == null)
            {
                continue;
            }

            var postedValues = posted.Values;

            for (var origValueIndex = 0; origValueIndex < original.Values.Count; origValueIndex++)
            {
                var originalValue = original.Values[origValueIndex];

                Models.ViewModels.FacetValue? postedValue = null;

                if (!string.IsNullOrEmpty(originalValue.Name))
                {
                    postedValue = postedValues.FirstOrDefault(v =>
                        string.Equals(v.Name, originalValue.Name, StringComparison.InvariantCultureIgnoreCase));
                }

                if (postedValue == null && origValueIndex < postedValues.Count)
                {
                    postedValue = postedValues[origValueIndex];
                }

                originalValue.Selected = postedValue?.Selected ?? false;
            }
        }

        AllFacets = fullFacets;
        Search.Facets = fullFacets;
    }

    private void SelectQualificationLevels()
    {
        const string name = nameof(SearchRequest.QualificationLevel);
        
        var facet = AllFacets
            .FirstOrDefault(f => f.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
        
        if (facet != null)
        {
            var selectedOptions = Search.QualificationLevelMap
                .Where(x => Search.QualificationLevels.Contains(x.Key))
                .SelectMany(x => x.Value)
                .Select(x => ((feat.common.Models.Enums.QualificationLevel)x).ToString())
                .ToHashSet();
            
            foreach (var value in facet.Values)
            {
                value.Selected = selectedOptions.Contains(value.Name);
            }
        }
    }
}
