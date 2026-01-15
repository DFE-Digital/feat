using feat.web.Enums;
using feat.web.Utils;

namespace feat.web.Models;

public class Search
{
    public List<string> History { get; set; } = [];
    public bool Updated { get; set; }
    
    public string? Location { get; set; } 
    public Distance? Distance { get; set; }
    public Distance? OriginalDistance { get; set; }
    public List<string> Interests { get; set; } = []; 
    public List<QualificationLevel> QualificationLevels { get; set; } = []; 
    public AgeGroup? AgeGroup { get; set; }
    
    public List<Models.ViewModels.Facet> Facets { get; set; } = [];
    
    public int CurrentPage { get; set; } = 1;
    public int TotalPages { get; set; }
    public int PageSize { get; set; } = 10;
    
    public OrderBy OrderBy { get; set; } = OrderBy.Relevance;
    
    public static Dictionary<QualificationLevel, int[]> QualificationLevelMap { get; } = new()
    {
        { QualificationLevel.None, [0] },
        { QualificationLevel.OneAndTwo, [1, 2] },
        { QualificationLevel.Three, [3] },
        { QualificationLevel.FourToEight, [4, 5, 6, 7, 8] }
    };
    
    private bool _pageIsChanging;

    private string Query => string.Join(", ", Interests.Select(i => i.ToLower().Trim()));
    
    public SearchRequest ToSearchRequest()
    {
        var request = new SearchRequest
        {
            Query = !string.IsNullOrWhiteSpace(Query) ? Query : "*",
            Page = CurrentPage,
            PageSize = PageSize,
            Location = Location,
            Radius = Distance.HasValue ? (int)Distance.Value : 1000,
            OrderBy = OrderBy,
            CourseType = GetSelectedFilters(nameof(SearchRequest.CourseType)),
            QualificationLevel = GetQualificationLevelFilters(),
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
            {
                return;
            }

            _pageIsChanging = true;

            if (!History.Contains(page))
            {
                if (History.Count > 0)
                {
                    BackPage = History.LastOrDefault();
                }

                History.Add(page);

                if (VisitedCheckAnswers)
                {
                    BackPage = History.FirstOrDefault();
                }

                if (page == PageName.CheckAnswers)
                {
                    VisitedCheckAnswers = true;
                }
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
        var facet = Facets
            .FirstOrDefault(f => f.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));

        return facet?.Values
            .Where(fv => fv.Selected)
            .Select(fv => fv.Name);
    }

    private IEnumerable<string> GetQualificationLevelFilters(List<Models.ViewModels.Facet>? facets = null)
    {
        const string name = nameof(SearchRequest.QualificationLevel);
        
        var facet = (facets ?? Facets)
            .FirstOrDefault(f => f.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));

        if (facet != null)
        {
            return facet.Values
                .Where(v => v.Selected)
                .Select(v => v.Name);
        }

        if (QualificationLevels.Count == 0)
        {
            return [];
        }
        
        return QualificationLevels
            .SelectMany(ql => QualificationLevelMap[ql])
            .Select(x => ((feat.common.Models.Enums.QualificationLevel)x).ToString());
    }
}
