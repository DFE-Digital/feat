using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using feat.ingestion.Models.FAC.Enums;

namespace feat.ingestion.Models.FAC.Converters;

internal sealed class AttendancePatternEnumConverter() : EnumConverter(typeof(AttendancePattern))
{
    public override object ConvertFromString(string? text, IReaderRow row, MemberMapData memberMapData)
    {
        if (string.IsNullOrEmpty(text))
            return AttendancePattern.Undefined;
        
        if(!Enum.TryParse(text?.ToUpper(), out AttendancePattern attendancePattern))
        {
            return AttendancePattern.Undefined;
        }

        // If we're set to day or block release, we need to set this to daytime
        // as per a NCS request
        if (attendancePattern == AttendancePattern.DayOrBlockRelease)
        {
            attendancePattern = AttendancePattern.Daytime;
        }
        
        return attendancePattern;
    }
}