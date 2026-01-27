using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;
using CsvHelper.Configuration;
using feat.common.Models.Enums;
using feat.ingestion.Models.FAC.Enums;

namespace feat.ingestion.Models.FAC;

[Table("FAC_Courses")]
public class Course
{
    [Key]
    public Guid CourseId { get; set; }
    
    public Status CourseStatus { get; set; }
    
    public DateTime CreatedOn { get; set; }
    
    public DateTime? UpdatedOn { get; set; }
    
    [StringLength(8)]
    public string LearnAimRef { get; set; }
    
    public int ProviderUkprn { get; set; }
    
    [StringLength(4000)]
    public string? CourseDescription { get; set; }
    
    [StringLength(4000)]
    public string? EntryRequirements { get; set; }
    
    [StringLength(4000)]
    public string? WhatYoullLearn { get; set; }
    
    [StringLength(4000)]
    public string? HowYoullLearn { get; set; }
    
    [StringLength(4000)]
    public string? WhatYoullNeed { get; set; }
    
    [StringLength(4000)]
    public string? HowYoullBeAssessed { get; set; }
    
    [StringLength(4000)]
    public string? WhereNext { get; set; }
    
    public CourseType? CourseType { get; set; }
    
    public EducationLevel? EducationLevel  { get; set; }
    
    [StringLength(500)]
    public string? AwardingBody { get; set; }
}

public sealed class CourseMap : ClassMap<Course>
{
    public CourseMap()
    {
        // Course Info
        Map(m => m.CourseId);
        Map(m => m.CourseStatus);
        Map(m => m.CreatedOn);
        Map(m => m.UpdatedOn).Default(new DateTime?(), useOnConversionFailure: true);
        Map(m => m.LearnAimRef);
        Map(m => m.ProviderUkprn);
        
        // String data, taking into account HTML encoding
        
        Map(m => m.CourseDescription).Convert(args => HtmlDecode(
            args.Row.GetField<bool?>("DataIsHtmlEncoded"),
            args.Row.GetField<string?>("CourseDescription")));
        
        Map(m => m.EntryRequirements).Convert(args => HtmlDecode(
            args.Row.GetField<bool?>("DataIsHtmlEncoded"),
            args.Row.GetField<string?>("EntryRequirements")));
        
        Map(m => m.WhatYoullLearn).Convert(args => HtmlDecode(
            args.Row.GetField<bool?>("DataIsHtmlEncoded"),
            args.Row.GetField<string?>("WhatYoullLearn")));
        
        Map(m => m.HowYoullLearn).Convert(args => HtmlDecode(
            args.Row.GetField<bool?>("DataIsHtmlEncoded"),
            args.Row.GetField<string?>("HowYoullLearn")));
        
        Map(m => m.WhatYoullNeed).Convert(args => HtmlDecode(
            args.Row.GetField<bool?>("DataIsHtmlEncoded"),
            args.Row.GetField<string?>("WhatYoullNeed")));
        
        Map(m => m.HowYoullBeAssessed).Convert(args => HtmlDecode(
            args.Row.GetField<bool?>("DataIsHtmlEncoded"),
            args.Row.GetField<string?>("HowYoullBeAssessed")));
        
        Map(m => m.WhereNext).Convert(args => HtmlDecode(
            args.Row.GetField<bool?>("DataIsHtmlEncoded"),
            args.Row.GetField<string?>("WhereNext")));
    }

    private static string? HtmlDecode(bool? dataIsHtmlEncoded, string? data)
    {
        if (data is null or "NULL" or "NA" or "N/A")
            return null;
        
        if (!dataIsHtmlEncoded.GetValueOrDefault(false)) 
            return data;
        
        var myWriter = new StringWriter();
        HttpUtility.HtmlDecode(data, myWriter);
        
        return myWriter.ToString();
    }
}