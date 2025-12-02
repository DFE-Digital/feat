using System.ComponentModel;
using System.Reflection;
using feat.common.Extensions.Attributes;

namespace feat.common.Extensions;

public static class EnumExtensions
{
    /// <summary>
    /// Gets an enum value of type T from its Description attribute value.
    /// If no Description matches, tries to match the enum name itself.
    /// </summary>
    public static T? GetValueFromDescription<T>(string description) where T : struct, Enum
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description must not be null or empty.", nameof(description));

        foreach (var field in typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static))
        {
            var attribute = field.GetCustomAttribute<DescriptionAttribute>();

            if ((attribute != null && attribute.Description.Equals(description, StringComparison.OrdinalIgnoreCase)) ||
                field.Name.Equals(description, StringComparison.OrdinalIgnoreCase))
            {
                return (T)field.GetValue(null)!;
            }
        }

        throw new ArgumentException($"No matching enum value found for description '{description}' in {typeof(T).Name}.");
    }
    
    public static int GetOrder(this Enum value)
    {
        var memberInfo = value.GetType().GetMember(value.ToString()).First();

        var orderAttribute = (OrderAttribute?)memberInfo
            .GetCustomAttributes(typeof(OrderAttribute), false)
            .FirstOrDefault();

        return orderAttribute?.Index ?? int.MaxValue;
    }
}
