using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using feat.api.Enums;

namespace feat.api.Models;

public class SearchRequest
{
    [Required]
    public required string Query { get; set; }
    
    public string? SessionId { get; set; } = Guid.NewGuid().ToString();
    
    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 10;
    
    public string? Location { get; set; }
    
    public double Radius { get; set; } = 1000;
    
    public bool IncludeOnlineCourses { get; set; } = false;
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public OrderBy OrderBy { get; set; } = OrderBy.Relevance;
}