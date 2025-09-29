using NetTopologySuite.Geometries;

namespace feat.ingestion.Models;

public class Location : BaseEntity
{
    public string? Name { get; set; } 

    public string? Address1 { get; set; } 

    public string? Address2 { get; set; } 

    public string? Address3 { get; set; }

    public string? Address4 { get; set; }

    public string? County { get; set; }

    public string? Email { get; set; }

    public Point? LocationPoint { get; set; } // Postgres PostGIS type 

    public string? Postcode { get; set; }

    public string? Telephone { get; set; } 

    public string? Town { get; set; }
    
    public string? Url { get; set; }
    
}