using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace feat.common.Models;

[Table("postcodelatlng")]
public class PostcodeLatLong
{
    [Key]
    [StringLength(8)]
    public required string Postcode { get; set; } 
    
    public double? Latitude { get; set; }
    
    public double? Longitude { get; set; }
    
}