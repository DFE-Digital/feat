using System.Text.Json.Serialization;

namespace feat.api.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum OrderBy
{
    Relevance,
    Distance
}