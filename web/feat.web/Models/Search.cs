using feat.web.Enums;
using feat.web.Utils;

namespace feat.web.Models;

public class Search
{
    //For navigation.
    public List<string> History { get; set; } = [];
    public bool Updated { get; set; } = true;
    
    public string? Location { get; set; } 
    public Distance? Distance { get; set; }
    public List<string> Interests { get; set; } = []; 
    public List<QualificationLevel> QualificationLevels { get; set; } = new(); 
    public AgeGroup? AgeGroup { get; set; }
    
    public List<Models.ViewModels.Facet> Facets { get; set; } = [];
    
    public bool Debug { get; set; } = false;
    
    // Store Pagination 
    public int CurrentPage { get; set; } = 1;
    public int TotalPages { get; set; }
    public int PageSize { get; set; } = 10;
    
    //Sorting: Distance | Relevance
    public OrderBy OrderBy { get; set; } = OrderBy.Relevance;
    
    private bool _pageIsChanging = false;

    private string Query => string.Join(", ", Interests.Select(i => i.ToLower().Trim()));
    
    public SearchRequest ToSearchRequest()
    {
        var request = new SearchRequest
        {
            Query = Query,
            Page = CurrentPage,
            PageSize = PageSize,
            Location = Location,
            Radius = Distance.HasValue ? (int)Distance.Value : 1000,
            OrderBy = OrderBy,
            EntryType = GetSelectedFilters(nameof(SearchRequest.EntryType)),
            QualificationLevel = GetSelectedFilters(nameof(SearchRequest.QualificationLevel)),
            LearningMethod = GetSelectedFilters(nameof(SearchRequest.LearningMethod)),
            CourseHours = GetSelectedFilters(nameof(SearchRequest.CourseHours)),
            StudyTime = GetSelectedFilters(nameof(SearchRequest.StudyTime))
        };

        return request;
    }

    public string? BackPage { get; set; }
    public bool VisitedCheckAnswers { get; set; }
    
    public void ResetHistory(string startPage)
    {
        History.Clear();
        History.Add(startPage);
    }

    public void SetPage(string page)
    {
        try
        {
            if (_pageIsChanging)
                return;
            _pageIsChanging = true;

            if (!History.Contains(page))
            {
                if (History.Count > 0)
                    BackPage = History.LastOrDefault();
                History.Add(page);
                
                if (VisitedCheckAnswers)
                    BackPage = History.FirstOrDefault();
                
                if (page == PageName.CheckAnswers) 
                    VisitedCheckAnswers = true;
            }
            else
            {
                if (!History.Contains(PageName.CheckAnswers))
                {
                    History.RemoveAt(History.Count - 1);
                    BackPage = History.Contains(page) ? History.ElementAt(History.Count - 2) : History.LastOrDefault();
                }
                else if (VisitedCheckAnswers)
                {
                    BackPage = History.LastOrDefault();
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            _pageIsChanging = false;
        }
    }
    
    private IEnumerable<string>? GetSelectedFilters(string name)
    {
        var facet = Facets.FirstOrDefault(f => f.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));

        return facet?.Values
            .Where(fv => fv.Selected)
            .Select(fv => fv.Name);
    }
}
