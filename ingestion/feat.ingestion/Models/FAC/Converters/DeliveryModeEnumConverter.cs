using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using feat.common.Models.Enums;

namespace feat.ingestion.Models.FAC.Converters;

internal sealed class DeliveryModeEnumConverter() : EnumConverter(typeof(DeliveryMode))
{
    public override object? ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
    {
        if (string.IsNullOrEmpty(text))
        {
            return null;
        }

        if (!Enum.TryParse(text.ToUpper(), out DeliveryMode deliveryMode))
        {
            return null;
        }

        return deliveryMode;
    }
}