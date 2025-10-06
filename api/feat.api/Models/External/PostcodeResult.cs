using System.Text.Json.Serialization;

namespace feat.api.Models.External;

public class PostcodeResult
{
    [JsonPropertyName("status")]
    public int? Status { get; set; }

    [JsonPropertyName("result")]
    public Postcode? Result { get; set; }
}