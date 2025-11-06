namespace feat.ingestion.Models.FAC.Enums;

public enum VenueStatus
{
    Live = 1,

    [Obsolete("App doesn't support Pending venues but still here because we have some data with this status")]
    Pending = 2,

    Archived = 4
}