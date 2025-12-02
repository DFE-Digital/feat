namespace feat.common.Extensions.Attributes;

[AttributeUsage(AttributeTargets.Field)]
public class OrderAttribute(int index) : Attribute
{
    public int Index { get; } = index;
}