using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace feat.ingestion.Models.FAC.Converters;

public class DDMMYYYY : DateTimeConverter
{
    public override object? ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
    {
        if (text is null or "NA" or "" or "NAN")
            return null;

        if (DateTime.TryParseExact(text, "dd/M/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal,
                out DateTime result))
        {
            return result;
        }

        return null;
    }
}