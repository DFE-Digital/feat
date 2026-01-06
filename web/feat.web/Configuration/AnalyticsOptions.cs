namespace feat.web.Configuration;

public class AnalyticsOptions
{
    public const string Name = "Analytics";
    public string GoogleTagManager { get; set; } = string.Empty;
    public string Clarity { get; set; } = string.Empty;
}