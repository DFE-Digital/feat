using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using feat.common.Extensions;
using feat.ingestion.Models.FAC.Enums;

namespace feat.ingestion.Models.FAC.Converters;

internal sealed class ApprovedQualificationLevelEnumConverter() : EnumConverter(typeof(ApprovedQualificationLevel))
{
    public override object ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
    {
        if (string.IsNullOrEmpty(text))
            return ApprovedQualificationLevel.Unknown;

        return EnumExtensions.GetValueFromDescription<ApprovedQualificationLevel>(text) ??
               ApprovedQualificationLevel.Unknown;
    }
}