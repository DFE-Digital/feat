using System.ComponentModel.DataAnnotations;
using feat.web.Enums;

namespace feat.web.Models;

public class Search
{
    public AgeGroup? AgeGroup { get; set; }
    
    public Distance? Distance { get; set; }
    
    public SearchMethod? SearchMethod { get; set; }
    
    public string? Location { get; set; }

    public double? Latitude { get; set; }
    
    public double? Longitude { get; set; }
    
    public int? MaxDistance { get; set; }
    
    public bool? JobOrCareers { get; set; }
    
    public List<string> JobOrCareersList { get; set; } = [];
    
    public bool? Subjects { get; set; }
    
    public List<string> SubjectsList { get; set; } = [];
    
    public CourseType? CourseType { get; set; }
    
    public CourseLevel? CourseLevel { get; set; }

}