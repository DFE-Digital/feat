namespace feat.api.Models;

public class AiSearchResult
{
    public required Guid Id { get; set; }
    
    public required Guid InstanceId { get; set; }
    
    public double? RerankerScore { get; set; }
}