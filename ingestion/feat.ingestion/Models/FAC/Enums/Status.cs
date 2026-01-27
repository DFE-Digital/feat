namespace feat.ingestion.Models.FAC.Enums;

[Flags]
public enum Status
{
    Live = 1,
    Deleted = 2,
    Archived = 4,
}