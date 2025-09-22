
namespace feat.ingestion.Models;

public class Base
{
    public Guid Id { get; set; }     
}

public class BaseEntity : Base
{
    public required DateTime Created { get; set; }

    public DateTime? Updated { get; set; } 
}