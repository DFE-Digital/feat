using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using feat.api.Enums;

namespace feat.api.Models;

public class SearchRequest
{
    [Required]
    [MaxLength(3)]
    public required string[] Query { get; set; }
    
    public string? SessionId { get; set; } = Guid.NewGuid().ToString();
    
    [Range(1, int.MaxValue, ErrorMessage = "Page must be at least 1.")]
    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 10;
    
    public string? Location { get; set; }
    
    public double Radius { get; set; } = 1000;
    
    public OrderBy OrderBy { get; set; } = OrderBy.Relevance;

    public IEnumerable<string>? CourseType { get; set; }

    public IEnumerable<string>? QualificationLevel { get; set; }

    public IEnumerable<string>? LearningMethod { get; set; }

    public IEnumerable<string>? CourseHours { get; set; }

    public IEnumerable<string>? StudyTime { get; set; }

    [JsonIgnore]
    protected internal string LuceneQuery
    {
        get
        {
            return Query.Length > 0 ? string.Join(" OR ", Query.Select(q => $"\"{q}\"")) : "*";
        }
    }
}