using System.Text.Json.Serialization;

namespace feat.api.Models.External;

public class Postcode
{
    [JsonPropertyName("postcode")]
    public string? Code { get; set; }

    [JsonPropertyName("quality")]
    public int? Quality { get; set; }

    [JsonPropertyName("longitude")]
    public double? Longitude { get; set; }

    [JsonPropertyName("latitude")]
    public double? Latitude { get; set; }
}