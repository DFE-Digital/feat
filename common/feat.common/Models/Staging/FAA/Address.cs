using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace feat.common.Models.Staging.FAA;

[Table("FAA_Addresses")]
public class Address
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [ForeignKey(nameof(Apprenticeship))]
    public int ApprenticeshipId { get; set; }
    
    public Apprenticeship Apprenticeship { get; set; }

    [StringLength(100)]
    public string? AddressLine1 { get; set; }
    
    [StringLength(100)]
    public string? AddressLine2 { get; set; }
    
    [StringLength(100)]
    public string? AddressLine3 { get; set; }
    
    [StringLength(100)]
    public string? AddressLine4 { get; set; }
    
    [StringLength(10)]
    public string? Postcode { get; set; }
    
    public double? Latitude { get; set; }
    
    public double? Longitude { get; set; }
}