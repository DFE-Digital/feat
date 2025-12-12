namespace feat.api.Models;

public record GeoLocationResponse(
    GeoLocation? Location,
    bool IsValid,
    string? ErrorMessage = null
);