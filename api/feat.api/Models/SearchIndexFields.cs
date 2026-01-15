using Microsoft.Spatial;

namespace feat.api.Models;

public class SearchIndexFields
{
    public required string Id { get; set; }
        
    public required string InstanceId { get; set; }
        
    public required string Title { get; set; }
        
    public required string LearningAimTitle { get; set; }
        
    public required string Description { get; set; }
        
    public required string Sector { get; set; }
        
    public required string EntryType { get; set; }
    
    public required string CourseType { get; set; }
        
    public required string QualificationLevel { get; set; }
        
    public required string LearningMethod { get; set; }
        
    public required string CourseHours { get; set; }
        
    public required string StudyTime  { get; set; }
        
    public required string Source { get; set; }
        
    public required GeographyPoint Location { get; set; }
}