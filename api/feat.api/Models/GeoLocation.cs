namespace feat.api.Models;

public class GeoLocation : IEquatable<GeoLocation>
{
    public double Latitude { get; set; }
    
    public double Longitude { get; set; }

    public bool Equals(GeoLocation? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Latitude.Equals(other.Latitude) && Longitude.Equals(other.Longitude);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((GeoLocation)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Latitude, Longitude);
    }
}