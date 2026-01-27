using Microsoft.Spatial;

namespace feat.api.Models;

public class SearchIndexFields
{
    public string Id { get; set; }
        
    public string InstanceId { get; set; }
        
    public string Title { get; set; }
        
    public string LearningAimTitle { get; set; }
        
    public string Description { get; set; }
        
    public string Sector { get; set; }
        
    public string EntryType { get; set; }
    
    public string CourseType { get; set; }
        
    public string QualificationLevel { get; set; }
        
    public string LearningMethod { get; set; }
        
    public string CourseHours { get; set; }
        
    public string StudyTime  { get; set; }
        
    public string Source { get; set; }
        
    public GeographyPoint Location { get; set; }
}