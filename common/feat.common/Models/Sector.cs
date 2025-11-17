using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices.JavaScript;
using feat.common.Models.Enums;

namespace feat.common.Models;

[Table("Sector")]
public class Sector
{
    [Key]
    public Guid Id { get; set; } 
    public SourceSystem? SourceSystem { get; set; }

    [StringLength(255)]
    public required string Name { get; set; } = string.Empty;

    public ICollection<EntrySector> EntrySectors { get; set; } = new List<EntrySector>();
}