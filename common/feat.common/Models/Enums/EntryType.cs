using System.Text.Json.Serialization;

namespace feat.common.Models.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum EntryType
{
    Course,
    Apprenticeship,
    UniversityCourse
}