using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace feat.ingestion.Models;

public class Base
{
    [Key]
    [Column(Order = 0)]
    public Guid Id { get; set; }     
}

public class BaseEntity : Base
{
    [Column(Order = 1)]
    public required DateTime Created { get; set; }

    [Column(Order = 2)]
    public DateTime? Updated { get; set; } 
}