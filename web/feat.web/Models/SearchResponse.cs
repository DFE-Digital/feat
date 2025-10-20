namespace feat.web.Models;

public class SearchResponse
{
    public int Page { get; set; }
    
    public int PageSize { get; set; }
    
    public long? TotalCount { get; set; }

    public List<Course> Courses { get; set; } = [];
    
    public IList<Facet> Facets { get; set; } = new List<Facet>();
    
}