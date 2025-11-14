using System.Text;
using feat.web.Enums;
using feat.web.Extensions;
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
    public List<string>? SelectedFilterFacetItems { get; set; } = [];  
    
    public bool IncludeOnlineCourses { get; set; } = true;
    public bool Debug { get; set; } = false;
    
    // Store Pagination 
    public int CurrentPage { get; set; } = 1;
    public int TotalPages { get; set; }
    public int PageSize { get; set; } = 10;
    
    //Sorting: Distance | Relevance
    public OrderBy OrderBy { get; set; }  = OrderBy.Relevance;
    
    // Course id for Details request
    public string? CourseId { get; set; }
    
    
    private bool _pageIsChanging = false;
    
    public string? Query {
        get
        {   
            // TODO List<QualificationLevel> and AgeGroup? -> Build up querry or send parameter?
              
            var selectedAgeGroup = AgeGroup.ToString();
            var selectedQualificationLevels = QualificationLevels.Select(x=> x.GetDisplayName()).ToList();

            var mergedList = new List<string>();
            Interests.ForEach((interest) =>
            {
                if (!string.IsNullOrEmpty(interest))
                    mergedList.Add(interest);
            });
            if (selectedAgeGroup != null) 
                mergedList.Add(selectedAgeGroup);

            if (selectedQualificationLevels.Count != 0)
                mergedList.AddRange(selectedQualificationLevels);
            
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
            CourseId = CourseId, //TODO Course detail request; possibly a different Api
            
        };
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
                
                if (page == PageName.CheckAnswers) 
                    VisitedCheckAnswers = true;
            }
            else
            {
                if (!History.Contains(PageName.CheckAnswers))
                {
                    History.RemoveAt(History.Count - 1);
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
}
