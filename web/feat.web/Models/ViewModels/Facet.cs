namespace feat.web.Models.ViewModels;

public class Facet
{
    public required string Name { get; set; }
    
    public required string DisplayName { get; set; }
    
    public int Index { get; set; }

    public List<FacetValue> Values { get; set; } = [];
}