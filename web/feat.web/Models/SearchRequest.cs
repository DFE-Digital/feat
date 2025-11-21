using feat.web.Enums;

namespace feat.web.Models;

public class SearchRequest
{
    public required string Query { get; set; }
    
    public string? SessionId { get; set; } = Guid.NewGuid().ToString();
    
    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 10;
    
    public string? Location { get; set; }
    
    public double Radius { get; set; } = 1000;
    
    public OrderBy OrderBy { get; set; } = OrderBy.Relevance;

    public IEnumerable<string>? EntryType { get; set; }

    public IEnumerable<string>? QualificationLevel { get; set; }

    public IEnumerable<string>? LearningMethod { get; set; }

    public IEnumerable<string>? CourseHours { get; set; }

    public IEnumerable<string>? StudyTime { get; set; }
}