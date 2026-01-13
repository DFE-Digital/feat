using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace feat.common.Models;

[Table("LocationLatLong")]
[Index(nameof(CleanName))]
public class LocationLatLong
{
    [Key]
    [StringLength(100)]
    public required string Name { get; set; } 
    
    [StringLength(100)]
    public required string CleanName { get; set; } 
    
    [StringLength(100)]
    public string? County { get; set; }
    
    public double? Latitude { get; set; }
    
    public double? Longitude { get; set; }
    
}