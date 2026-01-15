using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CsvHelper.Configuration;
using feat.ingestion.Models.FAC.Converters;
using feat.ingestion.Models.FAC.Enums;

namespace feat.ingestion.Models.FAC;

[Table("FAC_ApprovedQualifications")]
public class ApprovedQualification
{
    [StringLength(8)]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    [Key]
    public required string QualificationNumber { get; set; }
    
    public ApprovedQualificationLevel? Level { get; set; }

    public ApprovedQualificationType? QualificationType { get; set; }
    
    [StringLength(255)]
    public required string SectorSubjectArea { get; set; }
    
    public bool Age1416_FundingAvailable { get; set; }
    
    public bool Age1619_FundingAvailable { get; set; }
    
    public bool LocalFlexibilities_FundingAvailable { get; set; }
    
    public bool LegalEntitlementL2L3_FundingAvailable { get; set; }
    
    public bool LegalEntitlementEnglishandMaths_FundingAvailable { get; set; }
    
    public bool DigitalEntitlement_FundingAvailable { get; set; }
    
    public bool LifelongLearningEntitlement_FundingAvailable { get; set; }
    
    public bool AdvancedLearnerLoans_FundingAvailable { get; set; }
    
    public bool FreeCoursesForJobs_FundingAvailable { get; set; }
}

public sealed class ApprovedQualificationMap : ClassMap<ApprovedQualification>
{
    public ApprovedQualificationMap()
    {
        // Course Info
        Map(m => m.QualificationNumber);
        Map(m => m.Level).TypeConverter<ApprovedQualificationLevelEnumConverter>();
        Map(m => m.QualificationType).TypeConverter<ApprovedQualificationTypeEnumConverter>();
        Map(m => m.SectorSubjectArea);
        Map(m => m.Age1416_FundingAvailable);
        Map(m => m.Age1619_FundingAvailable);
        Map(m => m.LocalFlexibilities_FundingAvailable);
        Map(m => m.LegalEntitlementL2L3_FundingAvailable);
        Map(m => m.LegalEntitlementEnglishandMaths_FundingAvailable);
        Map(m => m.DigitalEntitlement_FundingAvailable);
        Map(m => m.LifelongLearningEntitlement_FundingAvailable);
        Map(m => m.AdvancedLearnerLoans_FundingAvailable);
        Map(m => m.FreeCoursesForJobs_FundingAvailable);
    }
}