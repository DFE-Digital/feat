using System.Text;
using feat.web.Enums;

namespace feat.web.Models;

public class Search
{
    //For navigation.
    public List<string> History { get; set; } = [];
    
    public bool Updated { get; set; } = true;
    
    
    public AgeGroup? AgeGroup { get; set; }
    
    public List<QualificationLevel> QualificationLevels { get; set; } = new();

    public Distance? Distance { get; set; }
    
    public SearchMethod? SearchMethod { get; set; }
    
    public SearchType? SearchType { get; set; }

    public bool IncludeOnlineCourses { get; set; } = true;
    
    public bool Debug { get; set; } = false;

    public string? Location { get; set; }

    public List<string> Interests { get; set; } = [];

    public List<string> Subjects { get; set; } = [];
    
    public List<string> Careers { get; set; } = [];
    
    public CourseType? CourseType { get; set; }
    
    public CourseLevel? CourseLevel { get; set; }
    
    // Store Pagination 
    public int CurrentPage { get; set; } = 1;
    public int TotalPages { get; set; }
    public int PageSize { get; set; } = 10;
    
    //Sorting Distance | Relevance
    public OrderBy OrderBy { get; set; }  = OrderBy.Relevance;
    
    // Course id for Details 
    public string? CourseId { get; set; }
    
    // private field to prevent re-entery
    private bool _pageIsChanging = false;
    private bool _pageBackIsChanging = false;
    
    public string? Query {
        get
        {
            var mergedList = Interests.Union(Subjects).Union(Careers).ToList();
            
            if (mergedList.Count == 0)
            {
                return "*";
            }
            
            var sb = new StringBuilder();
            foreach (var entry in mergedList.Distinct())
            {
                // If we have quotes in the terms, don't quite them
                if (entry.IndexOf('"') >= 0 && entry.LastIndexOf('"') < entry.Length - 1 && entry.IndexOf('"') < entry.LastIndexOf('"'))
                    sb.Append(entry + " ");
                // Otherwise, wrap the term in quotes
                else
                    sb.Append($"\"{entry}\" ");
            }
            
            return sb.ToString().Trim();
        }
    }

    public SearchRequest ToSearchRequest()
    {
        return new SearchRequest
        {
            Query = Query ?? string.Empty,
            IncludeOnlineCourses = IncludeOnlineCourses,
            Location = Location,
            Radius = Distance.HasValue ? (int)Distance.Value : 1000,
            OrderBy = OrderBy,
            PageNumber = CurrentPage,
            PageSize = PageSize, 
            Debug = Debug, 
            CourseId = CourseId
            
        };
    }

    public void SetPage(string page)
    {
        try
        {
            if (_pageIsChanging)
                return;
            _pageIsChanging = true;

            page = page.Trim().ToLower();
            if (!History.Contains(page))
            {
                History.Add(page);
            }
            else
            {
                var index = History.LastIndexOf(page);
                History.RemoveRange(index, History.Count - index);
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
    
    public string GetBackPage(string page)
    {
        try
        {
            if (_pageBackIsChanging)
                return string.Empty;
            _pageBackIsChanging = true;

            page = page.Trim().ToLower();
            if (!History.Contains(page))
            {
                return History.LastOrDefault() ?? "Index";
            }

            var index = History.LastIndexOf(page);
            if (index == 0)
                return "Index";

            return History[index - 1];
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        finally
        {
            _pageBackIsChanging = false;
        }
        return string.Empty;
    }

    public Search ClearSearch()
    {
        return new Search();
    }
}
