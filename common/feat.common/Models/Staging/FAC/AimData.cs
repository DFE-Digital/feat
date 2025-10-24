using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using feat.common.Models.Staging.FAC.Enums;

namespace feat.common.Models.Staging.FAC;

[Table("FAC_AimData")]
public class AimData
{
    [StringLength(8)]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [Key] public string LearnAimRef { get; set; }
    public EducationLevel? NotionalNVQLevelv2 { get; set; }
    [StringLength(255)]
    public string LearnAimRefTitle { get; set; }
    [StringLength(50)]
    public string AwardOrgCode { get; set; }
    
    
}

public sealed class AimDataMap : ClassMap<AimData>
{
    public AimDataMap()
    {
        // Course Info
        Map(m => m.LearnAimRef);
        Map(m => m.NotionalNVQLevelv2).Default(EducationLevel.Unknown, true);
        Map(m => m.LearnAimRefTitle);
        Map(m => m.AwardOrgCode);
    }
}

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