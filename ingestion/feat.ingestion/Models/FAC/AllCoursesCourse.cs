using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using CsvHelper.Configuration;
using feat.common.Extensions;
using feat.common.Models.Enums;
using feat.ingestion.Models.FAC.Converters;
using feat.ingestion.Models.FAC.Enums;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using DeliveryMode = feat.common.Models.Enums.DeliveryMode;

namespace feat.ingestion.Models.FAC;

[Table("FAC_AllCourses")]
[PrimaryKey(nameof(COURSE_ID), nameof(COURSE_RUN_ID))]
public class AllCoursesCourse
{
    public Guid COURSE_ID { get; set; }
    public Guid COURSE_RUN_ID { get; set; }
    public int PROVIDER_UKPRN { get; set; }
    [StringLength(255)] public string? PROVIDER_NAME { get; set; }
    [StringLength(8)] public string? LEARN_AIM_REF { get; set; }
    [StringLength(255)] public string? COURSE_NAME { get; set; }
    [StringLength(4000)] public string? WHO_THIS_COURSE_IS_FOR { get; set; }
    public DeliveryMode? DELIVER_MODE { get; set; }
    public StudyMode? STUDY_MODE { get; set; }
    public AttendancePattern? ATTENDANCE_PATTERN { get; set; }
    public bool? FLEXIBLE_STARTDATE { get; set; }
    public DateTime? STARTDATE { get; set; }
    [Column(TypeName = "bigint")]
    public TimeSpan? DURATION { get; set; }
    public decimal? COST { get; set; }
    [StringLength(2000)] public string? COST_DESCRIPTION { get; set; }
    public bool? NATIONAL { get; set; }
    public string[]? REGIONS { get; set; }
    [StringLength(255)] public string? LOCATION_NAME { get; set; }
    [StringLength(255)] public string? LOCATION_ADDRESS1 { get; set; }
    [StringLength(255)] public string? LOCATION_ADDRESS2 { get; set; }
    [StringLength(255)] public string? LOCATION_COUNTY { get; set; }
    [StringLength(255)] public string? LOCATION_EMAIL { get; set; }
    public Point? LOCATION { get; set; }
    [StringLength(10)] public string? LOCATION_POSTCODE { get; set; }
    [StringLength(255)] public string? LOCATION_TELEPHONE { get; set; }
    [StringLength(255)] public string? LOCATION_TOWN { get; set; }
    [StringLength(255)] public string? LOCATION_WEBSITE { get; set; }
    [StringLength(255)] public string? COURSE_URL { get; set; }
    public DateTime? UPDATED_DATE { get; set; }

    [StringLength(4000)] public string? ENTRY_REQUIREMENTS { get; set; }

    [StringLength(4000)] public string? HOW_YOU_WILL_BE_ASSESSED { get; set; }
    public CourseType? COURSE_TYPE { get; set; }
    [StringLength(255)] public string? SECTOR { get; set; }
    public EducationLevel? EDUCATION_LEVEL { get; set; }
    [StringLength(255)] public string? AWARDING_BODY { get; set; }
    public DateTime? CREATED_DATE { get; set; }
}

public sealed class AllCoursesCourseMap : ClassMap<AllCoursesCourse>
{
    public AllCoursesCourseMap()
    {
        // Course Info
        Map(m => m.COURSE_ID);
        Map(m => m.LEARN_AIM_REF);
        Map(m => m.COURSE_NAME).TypeConverter<CleanStringExtension>();
        Map(m => m.AWARDING_BODY).TypeConverter<CleanStringExtension>();
        Map(m => m.DELIVER_MODE);
        Map(m => m.COURSE_TYPE);
        Map(m => m.STUDY_MODE);
        Map(m => m.EDUCATION_LEVEL).Default(EducationLevel.Unknown, true);
        Map(m => m.ATTENDANCE_PATTERN).TypeConverter<AttendancePatternEnumConverter>();
        Map(m => m.FLEXIBLE_STARTDATE).Default(false, true);
        Map(m => m.DURATION).Convert(args => CalculateDuration(
            args.Row.GetField<DurationUnit?>("DURATION_UNIT"),
            args.Row.GetField<string?>("DURATION_VALUE"),
            args.Row.GetField<string?>("STARTDATE"))
        );
        Map(m => m.COURSE_URL).TypeConverter<CleanStringExtension>();

        // Dates
        Map(m => m.CREATED_DATE).TypeConverter<DDMMYYYY>();
        Map(m => m.UPDATED_DATE).TypeConverter<DDMMYYYY>();
        Map(m => m.STARTDATE).TypeConverter<DDMMYYYY>();

        // Cost
        Map(m => m.COST).Default(new decimal?(), useOnConversionFailure:true);
        Map(m => m.COST_DESCRIPTION).TypeConverter<CleanStringExtension>();

        // Text Data
        Map(m => m.WHO_THIS_COURSE_IS_FOR).TypeConverter<CleanStringExtension>();
        Map(m => m.ENTRY_REQUIREMENTS).TypeConverter<CleanStringExtension>();
        Map(m => m.HOW_YOU_WILL_BE_ASSESSED).TypeConverter<CleanStringExtension>();

        // Instance Info
        Map(m => m.COURSE_RUN_ID);

        // Provider Info
        Map(m => m.PROVIDER_UKPRN);
        Map(m => m.PROVIDER_NAME).TypeConverter<CleanStringExtension>();


        // Location Data
        Map(m => m.LOCATION_NAME).TypeConverter<CleanStringExtension>();
        Map(m => m.LOCATION_ADDRESS1).TypeConverter<CleanStringExtension>();
        Map(m => m.LOCATION_ADDRESS2).TypeConverter<CleanStringExtension>();
        Map(m => m.LOCATION_TOWN).TypeConverter<CleanStringExtension>();
        Map(m => m.LOCATION_COUNTY).TypeConverter<CleanStringExtension>();
        Map(m => m.LOCATION_POSTCODE).TypeConverter<CleanStringExtension>();
        Map(m => m.LOCATION).Convert(args => CalculateGeographyPoint(
            args.Row.GetField<string?>("LOCATION_LATITUDE"),
            args.Row.GetField<string?>("LOCATION_LONGITUDE")));
        Map(m => m.LOCATION_EMAIL).TypeConverter<CleanStringExtension>();
        Map(m => m.LOCATION_TELEPHONE).TypeConverter<CleanStringExtension>();
        Map(m => m.LOCATION_WEBSITE).TypeConverter<CleanStringExtension>();

        // Other Data
        Map(m => m.NATIONAL).Convert(args => 
            CalculateNational(
                args.Row.GetField<string?>("NATIONAL"), 
                args.Row.GetField<string?>("REGIONS"))
            );
        Map(m => m.REGIONS);
        Map(m => m.SECTOR).TypeConverter<CleanStringExtension>();
    }

    private bool? CalculateNational(string? national, string? regions)
    {
        if (!bool.TryParse(national, out var isNational))
        {
            isNational = false;
        }

        // If it is marked as regional, return that
        if (isNational || string.IsNullOrEmpty(regions)) return isNational;

        // Otherwise, check to see if we have regional sent as a region, and if so,
        // assume this is a regional course
        return regions
            .Split(',')
            .Any(region => region
                .Trim()
                .Equals("national", StringComparison.InvariantCultureIgnoreCase)) || isNational;
    }

    private static Point? CalculateGeographyPoint(string? latitudeText, string? longitudeText)
    {
        if (!double.TryParse(latitudeText, out var latitude) ||
            !double.TryParse(longitudeText, out var longitude))
        {
            return null;
        }
        
        // If we've got invalid details being passed, don't attempt to create a geographic point
        if (latitude == 0 || longitude == 0)
            return null;
        
        // Otherwise, create a point from the lat/long
        return new Point(new Coordinate(longitude, latitude)) { SRID = 4326 };
    }

    private static TimeSpan? CalculateDuration(DurationUnit? durationUnit, string? durationText,string? date)
    {
        if (!int.TryParse(durationText, out var duration))
        {
            duration = 0;
        }
        
        if (!DateTime.TryParseExact(date, "dd/M/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal,
                out DateTime start))
        {
            start = new DateTime(2025, 1, 1);
        }        
        DateTime end = durationUnit switch
        {
            DurationUnit.Days => start.AddDays(duration),
            DurationUnit.Weeks => start.AddDays(duration * 7),
            DurationUnit.Months => start.AddMonths(duration),
            DurationUnit.Years => start.AddYears(duration),
            DurationUnit.Hours => start.AddHours(duration),
            DurationUnit.Minutes => start.AddMinutes(duration),
            _ => start
        };

        return (end - start).Duration();
    }
}