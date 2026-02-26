using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using feat.api.Enums;

namespace feat.api.Models;
public class SearchRequest
{
    [Required]
    [MaxLength(3)]
    [Description("Up to three words or phrases to search for")]
    public required string[] Query { get; set; }
    
    [Description("A unique identifier to identify the source session to ensure consistent results")]
    public string? SessionId { get; set; } = Guid.NewGuid().ToString();
    
    [Range(1, int.MaxValue, ErrorMessage = "Page must be at least 1.")]
    [Description("The page number to retrieve the results")]
    public int Page { get; set; } = 1;

    [Description("The number of search results to display per page")]
    public int PageSize { get; set; } = 10;
    
    [Description("If latitude and longitude or a polygon are not passed, use this to search by location - postcode or location name including county")]
    public string? Location { get; set; }
    
    [Description("The latitude used for calculating distance and filtering results by radius. Prefer using this and the longitude over location name")]
    public double? Latitude { get; set; }
    
    [Description("The longitude used for calculating distance and filtering results by radius. Prefer using this and the latitude over location name")]
    public double? Longitude { get; set; }
    
    [MinLength(4)]
    [Description("A polygon within which to constrain the search results - must be at least 4 locations (with the first and last being the same)")]
    public List<GeoLocation>? LocationPolygon { get; set; }
    
    [Description("The radius to filter search results within (in miles). This should be set when searching by location.")]
    public double Radius { get; set; } = 1000;
    
    [Description("Whether to order the results by relevance (defualt) or distance")]
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