namespace feat.web.Models;

public class Facet
{
    public string Name { get; set; } = string.Empty;

    public Dictionary<string, long> Values { get; set; } = new();
}