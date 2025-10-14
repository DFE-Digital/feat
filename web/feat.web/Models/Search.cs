using System.Text;
using feat.web.Enums;

namespace feat.web.Models;

public class Search
{
    public bool Updated { get; set; } = true;
    
    public List<string> History { get; set; } = [];
    
    public AgeGroup? AgeGroup { get; set; }
    
    public QualificationLevel? QualificationLevel { get; set; }

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
            OrderBy = OrderBy.Relevance,
            Page = 1,
            PageSize = 20,
            Debug = Debug
        };
    }

    public void SetPage(string page)
    {
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

    public string GetBackPage(string page)
    {
        if (!History.Contains(page))
        {
            return History.LastOrDefault() ?? "Index";
        }

        var index = History.LastIndexOf(page);
        if (index == 0)
            return "Index";
        
        return History[index - 1];
    }
}