using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace feat.common.Models.Staging.FAC;

public class CleanString : StringConverter
{
    public override object? ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
    {
        var invalidValues = new[]
        {
            null,
            string.Empty,
            "NULL",
            "NA",
            ".",
            "N/A",
            "/",
            "\\"
        };
        
        return invalidValues.Contains(text?.Trim()) ? null : base.ConvertFromString(text, row, memberMapData);
    }
}