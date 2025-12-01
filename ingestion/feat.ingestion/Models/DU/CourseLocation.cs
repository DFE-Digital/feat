using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CsvHelper.Configuration;
using feat.ingestion.Models.DU.Enums;

namespace feat.ingestion.Models.DU;

[Table("DU_CourseLocations")]
public class CourseLocation
{
    public int UKPRN { get; set; }
    
    public int PubUKPRN { get; set; }

    [StringLength(50)]
    public required string CourseId { get; set; }
    
    public required StudyMode  StudyMode { get; set; }
    
    [StringLength(200)]
    public required string LocationId { get; set; }

}

public sealed class CourseLocationMap : ClassMap<CourseLocation>
{
    public CourseLocationMap()
    {
        // Course Info
        Map(m => m.UKPRN).Name("UKPRN");
        Map(m => m.PubUKPRN).Name("PUBUKPRN");
        Map(m => m.CourseId).Name("KISCOURSEID");
        Map(m => m.StudyMode).Name("KISMODE")
            .Default(StudyMode.Unknown, useOnConversionFailure: true);
        Map(m => m.LocationId).Name("LOCID");

        
    }
}