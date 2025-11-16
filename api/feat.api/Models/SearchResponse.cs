namespace feat.api.Models;

public class SearchResponse
{
    public int Page { get; set; }
    
    public int PageSize { get; set; }
    
    public int TotalCount { get; set; }

    public IEnumerable<Course> Courses { get; set; } = [];
    
    public IEnumerable<Facet> Facets { get; set; } = [];
}