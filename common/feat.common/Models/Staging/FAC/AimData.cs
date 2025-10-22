using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;
using CsvHelper.Configuration;
using feat.common.Models.Staging.FAC.Enums;

namespace feat.common.Models.Staging.FAC;

[Table("FAC_AimData")]
public class AimData
{
    [StringLength(8)]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [Key] public string LearnAimRef { get; set; }
    public EducationLevel? NotionalNVQLevel2 { get; set; }
    [StringLength(255)]
    public string LearnAimRefTitle { get; set; }
    [StringLength(10)]
    public string AwardOrgCode { get; set; }
    
    
}

public sealed class AimDataMap : ClassMap<AimData>
{
    public AimDataMap()
    {
        // Course Info
        Map(m => m.LearnAimRef);
        Map(m => m.NotionalNVQLevel2);
        Map(m => m.LearnAimRefTitle);
        Map(m => m.AwardOrgCode);
    }
}