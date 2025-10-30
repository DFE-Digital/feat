using feat.web.Models.ViewModels;

namespace feat.web.Models;

public class SearchResponse
{
    public int Page { get; set; }
    
    public int PageSize { get; set; }

    public long TotalCount { get; set; } = 0;

    public List<SearchResult> SearchResults { get; set; } = [];
    
    public IList<Facet> Facets { get; set; } = new List<Facet>();

    
    public string SortBy = "Distance";

}