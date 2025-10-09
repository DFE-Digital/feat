using Microsoft.Spatial;

namespace feat.api.Models;

public class AiSearchCourse
{
    public string id { get; set; }

    public string PROVIDER_NAME { get; set; }

    public string LOCATION { get; set; }

    public DateTimeOffset? START_DATE { get; set; }

    public string LOCATION_TOWN { get; set; }

    public string POSTCODE { get; set; }

    public string? COURSE_TYPE { get; set; }

    public string? SECTOR { get; set; }

    public string? ENTRY_REQUIREMENTS { get; set; }

    public string? DELIVERY_MODE { get; set; }

    public string? STUDY_MODE { get; set; }

    public string ATTENDANCE_MODE { get; set; }
    
    public string DURATION { get; set; }

    public string COST { get; set; }

    public string DATA_SOURCE { get; set; }

    public string EDUCATION_LEVEL { get; set; }

    public string APPLICATION_CLOSING_DATE { get; set; }

    public string HOURS_PER_WEEK { get; set; }

    public string WAGE { get; set; }

    public string WAGE_UNIT { get; set; }

    public string SKILLS_REQUIRED { get; set; }

    public string STANDARD_NAME { get; set; }

    public string WEBSITE { get; set; }

    public GeographyPoint? GEOPOINT_LATLONG { get; set; }

    public string empty_postcode_flag { get; set; }

    public string LEARNING_AIM_TITLE { get; set; }

    public string SSAT1 { get; set; }

    public string SSAT2 { get; set; }

    public string SKILLS_FOR_LIFE_DESC { get; set; }

    public string LEARNING_DIRECT_CLASSIFICATION { get; set; }

    public string TOPIC_MODELING { get; set; }

    public string AWARDING_BODY { get; set; }

    public string QUALIFICATION_TYPE { get; set; }

    public string LEVEL { get; set; }

    public string EMPLOYER_NAME { get; set; }

    public string COURSE_NAME { get; set; }
    
    public string WHO_THIS_COURSE_IS_FOR { get; set; }
}