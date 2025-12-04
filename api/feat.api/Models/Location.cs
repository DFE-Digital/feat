namespace feat.api.Models;

public class Location : IEquatable<Location>
{
    public string? Address1 { get; set; }
    
    public string? Address2 { get; set; }
    
    public string? Address3 { get; set; }
    
    public string? Address4 { get; set; }
    
    public string? Town { get; set; }
    
    public string? County { get; set; }
    
    public string? Postcode { get; set; }
    
    public GeoLocation? GeoLocation { get; set; }

    public bool Equals(Location? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Address1 == other.Address1 && Address2 == other.Address2 && Address3 == other.Address3 && Address4 == other.Address4 && Town == other.Town && County == other.County && Postcode == other.Postcode && Equals(GeoLocation, other.GeoLocation);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((Location)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Address1, Address2, Address3, Address4, Town, County, Postcode, GeoLocation);
    }
}