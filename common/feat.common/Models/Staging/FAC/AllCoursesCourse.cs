using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CsvHelper.Configuration;
using feat.common.Models.Staging.FAC.Enums;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace feat.common.Models.Staging.FAC;

[Table("FAC_AllCourses")]
[PrimaryKey(nameof(COURSE_ID), nameof(COURSE_RUN_ID))]
public class AllCoursesCourse
{
    public Guid COURSE_ID { get; set; }
    public Guid COURSE_RUN_ID { get; set; }
    public int PROVIDER_UKPRN { get; set; }
    [StringLength(255)] public string PROVIDER_NAME { get; set; }
    [StringLength(8)] public string? LEARN_AIM_REF { get; set; }
    [StringLength(255)] public string COURSE_NAME { get; set; }
    [StringLength(2000)] public string WHO_THIS_COURSE_IS_FOR { get; set; }
    public DeliveryMode? DELIVER_MODE { get; set; }
    public StudyMode? STUDY_MODE { get; set; }
    public AttendancePattern? ATTENDANCE_PATTERN { get; set; }
    public bool? FLEXIBLE_STARTDATE { get; set; }
    public DateTime? STARTDATE { get; set; }
    public TimeSpan? DURATION { get; set; }
    public decimal? COST { get; set; }
    [StringLength(255)] public string? COST_DESCRIPTION { get; set; }
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

    [StringLength(500)] public string? ENTRY_REQUIREMENTS { get; set; }

    [StringLength(500)] public string? HOW_YOU_WILL_BE_ASSESSED { get; set; }
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
        Map(m => m.COURSE_NAME);
        Map(m => m.AWARDING_BODY);
        Map(m => m.DELIVER_MODE);
        Map(m => m.COURSE_TYPE);
        Map(m => m.STUDY_MODE);
        Map(m => m.EDUCATION_LEVEL);
        Map(m => m.ATTENDANCE_PATTERN);
        Map(m => m.FLEXIBLE_STARTDATE);
        Map(m => m.DURATION).Convert(args => CalculateDuration(
            args.Row.GetField<DurationUnit?>("DURATION_UNIT"),
            args.Row.GetField<int>("DURATION_VALUE"),
            args.Row.GetField<DateTime?>("STARTDATE")));
        Map(m => m.COURSE_URL);

        // Dates
        Map(m => m.CREATED_DATE);
        Map(m => m.UPDATED_DATE);

        // Cost
        Map(m => m.COST);
        Map(m => m.COST_DESCRIPTION);

        // Text Data
        Map(m => m.WHO_THIS_COURSE_IS_FOR);
        Map(m => m.ENTRY_REQUIREMENTS);
        Map(m => m.HOW_YOU_WILL_BE_ASSESSED);

        // Instance Info
        Map(m => m.COURSE_RUN_ID);
        Map(m => m.STARTDATE);

        // Provider Info
        Map(m => m.PROVIDER_UKPRN);
        Map(m => m.PROVIDER_NAME);


        // Location Data
        Map(m => m.LOCATION_NAME);
        Map(m => m.LOCATION_ADDRESS1);
        Map(m => m.LOCATION_ADDRESS2);
        Map(m => m.LOCATION_TOWN);
        Map(m => m.LOCATION_COUNTY);
        Map(m => m.LOCATION_POSTCODE);
        Map(m => m.LOCATION).Convert(args => CalculateGeographyPoint(
            args.Row.GetField<double?>("LOCATION_LATITUDE"),
            args.Row.GetField<double?>("LOCATION_LONGITUDE")));
        Map(m => m.LOCATION_EMAIL);
        Map(m => m.LOCATION_TELEPHONE);
        Map(m => m.LOCATION_WEBSITE);

        // Other Data
        Map(m => m.NATIONAL);
        Map(m => m.REGIONS);
        Map(m => m.SECTOR);
    }

    private static Point? CalculateGeographyPoint(double? latitude, double? longitude)
    {
        // If we've got invalid details being passed, don't attempt to create a geographic point
        if (latitude == null || longitude == null || latitude.Value == 0 || longitude.Value == 0)
            return null;
        
        // Otherwise, create a point from the lat/long
        return new Point(new Coordinate(latitude.Value, longitude.Value));
    }

    private static TimeSpan? CalculateDuration(DurationUnit? durationUnit, int? durationValue, DateTime? startdate)
    {
        int duration = (int)Math.Abs(durationValue.GetValueOrDefault(0));
        DateTime start = startdate.GetValueOrDefault(new DateTime(1, 1, 2025));
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