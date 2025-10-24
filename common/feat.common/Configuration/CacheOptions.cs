using System.Diagnostics.CodeAnalysis;

namespace feat.common.Configuration;

[ExcludeFromCodeCoverage(Justification = "Configuration only")]
public class CacheOptions
{
    public static string Name => "Cache";
    
    public const string Memory = "Memory";
    public const string Redis = "Redis";

    public string Type { get; set; } = string.Empty;
    
    public string? ConnectionString { get; init; }

    public TimeSpan Duration { get; set; } = TimeSpan.FromDays(30);
}