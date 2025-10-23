using System.Diagnostics.CodeAnalysis;

namespace feat.ingestion.Configuration;

[ExcludeFromCodeCoverage(Justification = "Configuration only")]
public class IngestionOptions
{
    public static string Name => "Ingestion";
    public string Environment { get; set; } = string.Empty;
    public string ConnectionString { get; set; } = string.Empty;
    
    public string BlobStorageConnectionString { get; set; } = string.Empty;
    public string ApprenticeshipApiKey { get; set; } = string.Empty;
}