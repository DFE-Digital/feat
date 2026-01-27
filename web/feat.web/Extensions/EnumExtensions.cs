using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using feat.web.Utils;

namespace feat.web.Extensions;

public static class EnumExtensions
{
    public static string GetDescription(this Enum value)
    {
        var type = value.GetType();
        var member = type.GetMember(value.ToString()).FirstOrDefault();

        if (member == null)
        {
            return SharedStrings.NotProvided;
        }

        if (member.GetCustomAttribute<DisplayAttribute>() is { } displayAttribute)
        {
            if (!string.IsNullOrWhiteSpace(displayAttribute.Description))
            {
                return displayAttribute.Description;
            }

            if (!string.IsNullOrWhiteSpace(displayAttribute.Name))
            {
                return displayAttribute.Name;
            }
        }
        
        if (member.GetCustomAttribute<DescriptionAttribute>() is { } descriptionAttribute &&
            !string.IsNullOrWhiteSpace(descriptionAttribute.Description))
        {
            return descriptionAttribute.Description;
        }

        return value.ToString();
    }

    public static string GetDescription<TEnum>(this TEnum value) where TEnum : Enum
        => ((Enum)value).GetDescription();
    
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