using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace feat.common.Models;

[Table("PostcodeLatLong")]
public class PostcodeLatLong
{
    [Key]
    [StringLength(8)]
    public required string Postcode { get; set; } 
    
    public double? Latitude { get; set; }
    
    public double? Longitude { get; set; }
    
    public DateTime? Expired { get; set; }
    
    [NotMapped]
    public bool Valid => !Expired.HasValue ||  Expired.Value <= DateTime.Today;
}