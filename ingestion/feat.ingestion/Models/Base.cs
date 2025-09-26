using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace feat.ingestion.Models;

public class Base
{
    [Key]
    public Guid Id { get; set; }     
}

public class BaseEntity : Base
{
    [Column(TypeName = "datetime", Order = 1)]
    public required DateTime Created { get; set; }

    [Column(TypeName = "datetime", Order = 2)]
    public DateTime? Updated { get; set; } 
}