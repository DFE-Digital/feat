namespace feat.web.Models;

public class Facet
{
    public required string Name { get; set; }

    public Dictionary<string, long> Values { get; set; } = new();
}