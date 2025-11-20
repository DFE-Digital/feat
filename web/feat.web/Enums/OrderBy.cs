using System.Text.Json.Serialization;

namespace feat.web.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum OrderBy
{
    Relevance,
    Distance
}