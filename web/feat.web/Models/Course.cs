using System.Text.Json;
using System.Text.Json.Serialization;
using feat.common.Models.Enums;
using feat.web.Extensions;

namespace feat.web.Models;

public class Course
{
    public required string CourseId { get; set; } 
    public decimal DistanceSudo { get; set; } = 1;
    
    public required string CourseTitle { get; init; }
    
    public string? ProviderName { get; init; }
    
    public string? Location { get; init; }
    
    public string? Distance { get; init; }
    public CourseType? CourseType { get; init; }
    public string CourseTypeDisplay => CourseType?.GetDescription() ?? "Not available";
    
    public string? Requirements { get; init; }
    
    public string? Overview { get; init; }
    
    public string ToJson()
    {
        var json = JsonSerializer.Serialize(this, new JsonSerializerOptions()
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
            Converters = { new JsonStringEnumConverter() }
        });

        return json;
    }
}