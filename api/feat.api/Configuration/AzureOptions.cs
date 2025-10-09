namespace feat.api.Configuration;

public class AzureOptions
{
    public string OpenAiKey { get; set; } = string.Empty;
    
    public string OpenAiEndpoint { get; set; } = string.Empty;
    
    public string AiSearchKey { get; set; } = string.Empty;
    
    public string AiSearchUrl { get; set; } = string.Empty;
    
    public string AiSearchIndex { get; set; } = string.Empty;

    public string AiSearchIndexScoringProfile { get; set; } = string.Empty;
    
    public string AiSearchIndexScoringParameters { get; set; } = string.Empty;

    public float? Weight { get; set; } = 1.25f;
    
    public int? Knn { get; set; } = 125;
}