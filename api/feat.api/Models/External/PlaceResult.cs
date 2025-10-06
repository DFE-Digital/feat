using System.Text.Json.Serialization;

namespace feat.api.Models.External;

public class PlaceResult
{
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    [JsonPropertyName("result")]
    public List<Place>? Result { get; set; }
}