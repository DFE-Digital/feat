namespace feat.web.Configuration;

public class SearchOptions
{
    public const string Search = "Search";
    
    public string ApiKey { get; set; } = string.Empty;
    
    public string Url { get; set; } = string.Empty;
}