namespace feat.web.Models;

public class ClientFacet
{
    public string Name { get; set; } = string.Empty;
    public Dictionary<string, long> Values  { get; set; } = new();
    public string Description => Name.Contains('_') ? Name.Replace('_', ' ') : Name;
}