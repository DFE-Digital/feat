using System.Diagnostics.CodeAnalysis;

namespace feat.ingestion.Configuration;

[ExcludeFromCodeCoverage(Justification = "Configuration only")]
public class IngestionOptions
{
    public static string Name => "Ingestion";
    public string Environment { get; set; } = string.Empty;
    public string ApprenticeshipApiKey { get; set; } = string.Empty;
    
    public bool IndexDirectly { get; set; } = true;
}