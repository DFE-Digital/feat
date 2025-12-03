namespace feat.common.Extensions;

public static class TimeSpanExtensions
{
    public static double TotalYears(this TimeSpan duration)
    {
        return duration.TotalDays / 365;
    }
    
    public static double TotalMonths(this TimeSpan duration)
    {
        return duration.TotalYears() * 12;
    }
}