using System.Text.Json.Serialization;

namespace feat.api.Models.External;

public class Place
{
    [JsonPropertyName("code")]
    public string? Code { get; set; }

    [JsonPropertyName("name_1")]
    public string? Name1 { get; set; }

    [JsonPropertyName("name_2")]
    public object? Name2 { get; set; }

    [JsonPropertyName("longitude")]
    public double? Longitude { get; set; }

    [JsonPropertyName("latitude")]
    public double? Latitude { get; set; }
}