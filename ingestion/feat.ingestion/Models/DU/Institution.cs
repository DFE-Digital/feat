using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CsvHelper.Configuration;
using feat.ingestion.Models.DU.Enums;

namespace feat.ingestion.Models.DU;

[Table("DU_Institutions")]
public class Institution
{
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int UKPRN { get; set; }
    
    public int PubUKPRN { get; set; }
    
    [StringLength(300)]
    public required string Name { get; set; }
    
    [StringLength(300)]
    public string? TradingName { get; set; }
    
    [StringLength(300)]
    public string? OtherNames { get; set; }
    
    [StringLength(300)]
    public string? Address { get; set; }
    
    [StringLength(1000)]
    public string? Url { get; set; }
    
    [StringLength(100)]
    public string? Telephone { get; set; }

    public RegionCode Country { get; set; }
    
    [StringLength(1000)]
    public string? StudentUnionUrl { get; set; }

}

public sealed class InstitutionMap : ClassMap<Institution>
{
    public InstitutionMap()
    {
        // Course Info
        Map(m => m.UKPRN).Name("UKPRN");
        Map(m => m.PubUKPRN).Name("PUBUKPRN");
        Map(m => m.Name).Name("LEGAL_NAME");
        Map(m => m.TradingName).Name("FIRST_TRADING_NAME");
        Map(m => m.OtherNames).Name("OTHER_NAMES");
        Map(m => m.Address).Name("PROVADDRESS");
        Map(m => m.Telephone).Name("PROVTEL");
        Map(m => m.Url).Name("PROVURL");
        Map(m => m.Country).Name("COUNTRY");
        Map(m => m.StudentUnionUrl).Name("SUURL");
    }
}