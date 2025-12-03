using feat.common.Models.Enums;
using feat.web.Extensions;
using feat.web.Utils;

namespace feat.web.Models.ViewModels;

public abstract class CourseDetailsBase
{
    public string? Title { get; init; }
    
    public EntryType? EntryType { get; init; }
    
    public CourseType? CourseType { private get; init; }
    
    public string CourseTypeDisplay => CourseType?.GetDescription() ?? NotProvidedString;
    
    public int? Level { get; init; }
    
    public string? EntryRequirements { get; init; }
    
    public string? Description { get; init; }
    
    public string? WhatYouWillLearn { get; set; }

    public DeliveryMode? DeliveryMode { private get; init; }
    
    public string DeliveryModeDisplay => DeliveryMode?.GetDescription() ?? NotProvidedString;
    
    public TimeSpan? Duration { private get; init; }

    public string DurationDisplay => FormatDuration(Duration);

    public CourseHours? Hours { private get; init; }

    public string HoursDisplay => Hours?.GetDescription() ?? NotProvidedString;
    
    public string? CourseUrl { get; init; }

    protected static string NotProvidedString => SharedStrings.NotProvided;
    
    private static string FormatDuration(TimeSpan? duration)
    {
        if (duration == null)
        {
            return NotProvidedString;
        }

        var totalDays = duration.Value.TotalDays;
        var totalMonths = (int)(totalDays / 30);
        var totalYears = totalMonths / 12;
        var remainingMonths = totalMonths % 12;
        var remainingDays = (int)(totalDays % 30);

        var parts = new List<string>();

        if (totalYears > 0)
        {
            parts.Add($"{totalYears} year{(totalYears == 1 ? "" : "s")}");
        }

        if (remainingMonths > 0)
        {
            parts.Add($"{remainingMonths} month{(remainingMonths == 1 ? "" : "s")}");
        }

        if (parts.Count == 0 || (remainingDays > 0 && totalMonths < 1))
        {
            parts.Add($"{remainingDays} day{(remainingDays == 1 ? "" : "s")}");
        }

        return string.Join(" ", parts);
    }
}