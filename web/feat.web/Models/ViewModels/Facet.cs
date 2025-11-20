namespace feat.web.Models.ViewModels;

public class Facet
{
    public string Name { get; set; }

    public Dictionary<string, bool> Values { get; set; } = new();
}