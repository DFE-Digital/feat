using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace feat.web.Extensions;

public static class EnumExtesions
{
    /// <summary>
    /// If <paramref name="enum"/> has <see cref="DisplayAttribute"/> defined, this will return <see cref="DisplayAttribute.Name"/>. Otherwise, <see langword="null" /> will be returned.
    /// </summary>
    /// <returns><see cref="string"/> containing <see cref="DisplayAttribute.Name"/> if defined. Otherwise, will return <see langword="null" /></returns>
    public static string? GetDisplayName<T>(this T @enum) where T : Enum
        => @enum.GetEnumAttribute<T, DisplayAttribute>()?.Name;
    /// <summary>
    /// If <paramref name="enum"/> has <see cref="DisplayAttribute"/> defined, this will return <see cref="DisplayAttribute.Description"/>. Otherwise, <see langword="null" /> will be returned.
    /// </summary>
    /// <returns><see cref="string"/> containing <see cref="DisplayAttribute.Description"/> if defined. Otherwise, will return <see langword="null" /></returns>
    public static string? GetDescription<T>(this T @enum) where T : Enum
        => @enum.GetEnumAttribute<T, DisplayAttribute>()?.Description;
    /// <summary>
    /// Returns <see cref="DisplayAttribute"/> from <paramref name="enum"/> if defined. Otherwise will return <see langword="null" />.
    /// </summary>
    public static DisplayAttribute? GetDisplayAttribute<T>(this T @enum) where T : Enum
        => @enum.GetEnumAttribute<T, DisplayAttribute>();
    private static TAttribute? GetEnumAttribute<TEnum, TAttribute>(this TEnum @enum)
        where TEnum : Enum
        where TAttribute : Attribute
    {
        var memberInfo = typeof(TEnum).GetMember(@enum.ToString());
        return memberInfo[0].GetCustomAttribute<TAttribute>();
    }
}