using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using feat.common.Extensions;
using feat.ingestion.Models.FAC.Enums;

namespace feat.ingestion.Models.FAC.Converters;

internal sealed class ApprovedQualificationTypeEnumConverter() : EnumConverter(typeof(ApprovedQualificationType))
{
    public override object ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
    {
        if (string.IsNullOrEmpty(text))
            return ApprovedQualificationType.Unknown;

        return EnumExtensions.GetValueFromDescription<ApprovedQualificationType>(text) ??
               ApprovedQualificationType.Unknown;
    }
}