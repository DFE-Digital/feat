using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace feat.web.Extensions;

public static class EnumExtensions
{
    public static string GetDescription<TEnum>(this TEnum value) where TEnum : Enum
    {
        var memberInfo = typeof(TEnum).GetMember(value.ToString());
        
        if (memberInfo.Length == 0)
        {
            return value.ToString();
        }

        var member = memberInfo[0];
        
        if (member.GetCustomAttribute<DisplayAttribute>() is { } displayAttr)
        {
            if (!string.IsNullOrWhiteSpace(displayAttr.Description))
            {
                return displayAttr.Description;
            }

            if (!string.IsNullOrWhiteSpace(displayAttr.Name))
            {
                return displayAttr.Name;
            }
        }
        
        if (member.GetCustomAttribute<DescriptionAttribute>() is { } descAttr &&
            !string.IsNullOrWhiteSpace(descAttr.Description))
        {
            return descAttr.Description;
        }
        
        return value.ToString();
    }
    
    public static string GetDisplayName<TEnum>(this TEnum value) where TEnum : Enum
    {
        var memberInfo = typeof(TEnum).GetMember(value.ToString());

        if (memberInfo.Length == 0)
        {
            return value.ToString();
        }

        var member = memberInfo[0];

        if (member.GetCustomAttribute<DisplayAttribute>() is { } displayAttr &&
            !string.IsNullOrWhiteSpace(displayAttr.Name))
        {
            return displayAttr.Name;
        }
        
        return value.ToString();
    }
    
    public static TAttribute? GetAttribute<TEnum, TAttribute>(this TEnum value)
        where TEnum : Enum
        where TAttribute : Attribute
    {
        var memberInfo = typeof(TEnum).GetMember(value.ToString());

        return memberInfo.Length == 0 ? null : memberInfo[0].GetCustomAttribute<TAttribute>();
    }
}