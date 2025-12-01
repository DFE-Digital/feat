using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CsvHelper.Configuration;
using feat.ingestion.Models.DU.Enums;

namespace feat.ingestion.Models.DU;

[Table("DU_Courses")]
public class Course
{
    public int UKPRN { get; set; }
    public int PubUKPRN { get; set; }

    [StringLength(50)]
    public required string CourseId { get; set; }
    
    public required StudyMode  StudyMode { get; set; }
    
    [StringLength(255)]
    public required string Title { get; set; }
    
    [StringLength(255)]
    public string? AssessmentUrl { get; set; }    
    
    [StringLength(255)]
    public string? CostsUrl { get; set; }
    
    [StringLength(255)]
    public string? CourseUrl { get; set; }
    
    [StringLength(255)]
    public string? LearningUrl { get; set; }
    
    public Availability? DistanceLearning { get; set; }
    
    public Availability? FoundationYear { get; set; }
    
    public bool? Honours { get; set; }
    
    public bool? NHS { get; set; }
    
    public Availability? Sandwich { get; set; }
    
    public Availability? YearAbroad { get; set; }
    
    public int? NumberOfYears { get; set; }
    
   
    
    public int? Hecos { get; set; }
    public int? Hecos2 { get; set; }
    public int? Hecos3 { get; set; }
    public int? Hecos4 { get; set; }
    public int? Hecos5 { get; set; }
    
    public int? Aim { get; set; }
    

}

public sealed class CourseMap : ClassMap<Course>
{
    public CourseMap()
    {
        // Course Info
        Map(m => m.UKPRN);
        Map(m => m.PubUKPRN).Name("PUBUKPRN");
        Map(m => m.CourseId).Name("KISCOURSEID");
        Map(m => m.StudyMode).Name("KISMODE")
            .Default(StudyMode.Unknown, useOnConversionFailure: true);
        Map(m => m.Title).Name("TITLE");
        Map(m => m.AssessmentUrl).Name("ASSURL");
        Map(m => m.CostsUrl).Name("CRSECSTURL");
        Map(m => m.CourseUrl).Name("CRSEURL");
        Map(m => m.LearningUrl).Name("LTURL");
        
        Map(m => m.DistanceLearning).Name("DISTANCE")
            .Default(Availability.NotAvailable, useOnConversionFailure: true);
        Map(m => m.FoundationYear).Name("FOUNDATION")
            .Default(Availability.NotAvailable, useOnConversionFailure: true);
        Map(m => m.Sandwich).Name("SANDWICH")
            .Default(Availability.NotAvailable, useOnConversionFailure: true);
        Map(m => m.YearAbroad).Name("YEARABROAD")
            .Default(Availability.NotAvailable, useOnConversionFailure: true);
        
        Map(m => m.NumberOfYears).Name("NUMSTAGE");
        Map(m => m.Honours).Name("HONOURS");
        Map(m => m.NHS).Name("NHS");
        
        Map(m => m.Hecos).Name("HECOS").NameIndex(0);
        Map(m => m.Hecos2).Name("HECOS").NameIndex(1);
        Map(m => m.Hecos3).Name("HECOS").NameIndex(2);
        Map(m => m.Hecos4).Name("HECOS").NameIndex(3);
        Map(m => m.Hecos5).Name("HECOS").NameIndex(4);
        
        Map(m => m.Aim).Name("KISAIMCODE");
        
    }
}