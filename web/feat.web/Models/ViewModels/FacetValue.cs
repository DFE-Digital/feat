namespace feat.web.Models.ViewModels;

public class FacetValue
{
    public required string Name { get; set; }
    
    public required string DisplayName { get; set; }

    public bool Available { get; set; }
    
    public bool Selected { get; set; }
    
    public int Index { get; set; }  
}