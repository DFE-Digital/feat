using feat.common.Extensions;
using feat.common.Models.Enums;
using feat.web.Extensions;
using feat.web.Utils;

namespace feat.web.Models.ViewModels;

public abstract class CourseDetailsBase
{
    public string? Title { get; init; }
    
    public EntryType? EntryType { get; init; }
    
    public CourseType? CourseType { private get; init; }
    
    public string CourseTypeDisplay => CourseType == common.Models.Enums.CourseType.Unknown ? NotProvidedString :
        CourseType?.GetDescription() ?? NotProvidedString;
    
    public int? Level { get; init; }

    public string LevelDisplay => Level switch
    {
        null => NotProvidedString,
        0 => "Entry level",
        _ => Level.ToString()
    } ?? NotProvidedString;
    
    public string? EntryRequirements { get; init; }
    
    public string? Description { get; init; }
    
    public string? FullDescription { get; init; }
    
    public string? WhatYouWillLearn { get; set; }

    public DeliveryMode? DeliveryMode { private get; init; }
    
    public string DeliveryModeDisplay => DeliveryMode?.GetDescription() ?? NotProvidedString;
    
    public TimeSpan? Duration { private get; init; }

    public string DurationDisplay => FormatDuration(Duration);

    public CourseHours? Hours { private get; init; }

    public string HoursDisplay => Hours?.GetDescription() ?? NotProvidedString;
    
    public string? CourseUrl { private get; init; }
    
    public string? CourseUrlAbsolute => NormalizeUrl(CourseUrl);

    protected static string NotProvidedString => SharedStrings.NotProvided;
    
    protected static string? NormalizeUrl(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return null;
        }

        input = input.Trim();

        if (Uri.TryCreate(input, UriKind.Absolute, out var absolute))
        {
            return absolute.ToString();
        }

        return Uri.TryCreate($"https://{input}", UriKind.Absolute, out var https)
            ? https.ToString()
            : null;
    }
    
    private static string FormatDuration(TimeSpan? duration)
    {
        if (duration == null ||  duration == TimeSpan.Zero ||  duration == TimeSpan.MinValue)
        {
            return NotProvidedString;
        }
        
        var totalMonths = (int)duration.Value.TotalMonths();
        var totalYears = (int)duration.Value.TotalYears();
        var remainingMonths = (int)duration.Value.TotalMonths() % 12;
        var remainingDays = (int)duration.Value.TotalDays % 30;
        var remainingHours = (int)duration.Value.TotalHours % 24;

        var parts = new List<string>();

        if (totalYears > 0)
        {
            parts.Add($"{totalYears} year{(totalYears == 1 ? "" : "s")}");
        }

        if (remainingMonths > 0)
        {
            parts.Add($"{remainingMonths} month{(remainingMonths == 1 ? "" : "s")}");
        }
        
        if (parts.Count == 0 && remainingDays > 0 && totalMonths < 1)
        {
            parts.Add($"{remainingDays} day{(remainingDays == 1 ? "" : "s")}");
        }

        if (parts.Count == 0 && remainingHours is > 0 and < 24)
        {
            parts.Add($"{remainingHours} hour{(remainingHours == 1 ? "" : "s")}");
        }

        return string.Join(" ", parts);
    }
}