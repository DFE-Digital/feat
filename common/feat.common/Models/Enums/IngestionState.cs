namespace feat.common.Models.Enums;

public enum IngestionState
{
    Pending = 0,
    Processing = 1,
    ProcessingGeolocation = 4,
    Complete = 2,
    Failed = 3
}