using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace feat.common.Models;

[Table("LocationLatLong")]
public class LocationLatLong
{
    [Key]
    [StringLength(100)]
    public required string Name { get; set; } 
    
    public double? Latitude { get; set; }
    
    public double? Longitude { get; set; }
    
}