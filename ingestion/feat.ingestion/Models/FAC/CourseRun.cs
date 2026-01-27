using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CsvHelper.Configuration;
using feat.common.Extensions;
using feat.ingestion.Models.FAC.Converters;
using feat.ingestion.Models.FAC.Enums;
using DeliveryMode = feat.ingestion.Models.FAC.Enums.DeliveryMode;

namespace feat.ingestion.Models.FAC;

[Table("FAC_CourseRuns")]
public class CourseRun
{
    [Key]
    public Guid CourseRunId { get; set; }
    
    public Guid CourseId { get; set; }
    
    public Status CourseRunStatus { get; set; }
    
    public DateTime CreatedOn { get; set; }
    
    public DateTime? UpdatedOn { get; set; }

    public Guid? VenueId { get; set; }
    
    public DeliveryMode DeliveryMode { get; set; }
    
    public bool FlexibleStartDate { get; set; }
    
    public DateTime? StartDate { get; set; }
    
    public string? CourseWebsite {  get; set; }
    
    public decimal? Cost { get; set; }
    
    public string? CostDescription {  get; set; }
    
    [Column(TypeName = "bigint")]
    public TimeSpan? Duration { get; set; }
    
    public StudyMode? StudyMode { get; set; }
    
    public AttendancePattern? AttendancePattern { get; set; }
    
    public bool National { get; set; }
}

public sealed class CourseRunMap : ClassMap<CourseRun>
{
    public CourseRunMap()
    {
        Map(m => m.CourseId);
        Map(m => m.CourseRunId);
        Map(m => m.CreatedOn);
        Map(m => m.UpdatedOn).Default(new DateTime?(), useOnConversionFailure: true);
        Map(m => m.CourseRunStatus).Default(Status.Live, useOnConversionFailure: true);
        Map(m => m.VenueId).Default(new Guid?(), useOnConversionFailure: true);
        Map(m => m.DeliveryMode).Default(DeliveryMode.ClassroomBased, useOnConversionFailure: true);
        Map(m => m.FlexibleStartDate).Default(false, useOnConversionFailure: true);
        Map(m => m.StartDate).TypeConverter<DDMMYYYY>();
        Map(m => m.Cost).Default(new decimal?(), useOnConversionFailure:true);
        Map(m => m.CostDescription).TypeConverter<CleanStringExtension>();
        Map(m => m.CourseWebsite).TypeConverter<CleanStringExtension>();
        Map(m => m.AttendancePattern).TypeConverter<AttendancePatternEnumConverter>();
        Map(m => m.National).Default(false, useOnConversionFailure: true);
    }
}