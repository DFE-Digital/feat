using System.Text.Json.Serialization;
using feat.ingestion.Models.FAA.External.Enums;

namespace feat.ingestion.Models.FAA.External;

public class Qualification
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public QualificationWeighting? Weighting { get; set; }
    
    public string? QualificationType { get; set; }
    
    public string? Subject { get; set; }
    
    public string? Grade { get; set; }
}