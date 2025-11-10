using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using feat.ingestion.Models.FAC.Enums;

namespace feat.ingestion.Models.FAC.Converters;

internal sealed class EducationLevelEnumConverter() : EnumConverter(typeof(EducationLevel))
{
    public override object ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
    {
        if (string.IsNullOrEmpty(text))
            return EducationLevel.Unknown;
        
        if(!Enum.TryParse(text?.ToUpper(), out EducationLevel levelType))
        {
            return EducationLevel.Unknown;
        }

        return levelType;
    }
}