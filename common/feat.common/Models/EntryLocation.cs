using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using feat.common.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace feat.common.Models;

[Table("EntryLocation")]
public class EntryLocation
{
    public Guid EntryId { get; set; }

    public Guid LocationId { get; set; }

    public SourceSystem? SourceSystem { get; set; }
    
    [ForeignKey("EntryId")]
    [InverseProperty("EntryLocations")]
    public Entry Entry { get; set; } = null!;

    [ForeignKey("LocationId")]
    [InverseProperty("EntryLocations")]
    public Location Location { get; set; } = null!;
}
