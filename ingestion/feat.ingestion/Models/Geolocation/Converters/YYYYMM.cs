using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace feat.ingestion.Models.Geolocation.Converters;

public class YYYYMM : DateTimeConverter
{
    public override object? ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
    {
        if (string.IsNullOrEmpty(text))
            return null;

        var year = Convert.ToInt32(text.Substring(0, 4));
        var month = Convert.ToInt32(text.Substring(4, 2));
        
        return new  DateTime(year, month, 1);
    }
}