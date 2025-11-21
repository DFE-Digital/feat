namespace feat.web.Models.ViewModels;

public class Facet
{
    public string Name { get; set; }
    
    public string DisplayName { get; set; }
    
    public int Index { get; set; }

    public List<FacetValue> Values { get; set; } = [];
}