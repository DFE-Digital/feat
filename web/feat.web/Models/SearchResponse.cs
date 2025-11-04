using feat.web.Enums;
using feat.web.Models.ViewModels;

namespace feat.web.Models;

public class SearchResponse
{
    public int Page { get; init; }
    
    public int PageSize { get; init; }

    public long TotalCount { get; init; } = 0;

    public List<SearchResult> SearchResults { get; init; } = [];
    
    public List<Facet> Facets { get; set; } = new List<Facet>();

    public OrderBy OrderBy { get; set; }

    public object? CourseDetails { get; init; }
}