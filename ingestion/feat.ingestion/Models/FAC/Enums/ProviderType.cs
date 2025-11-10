namespace feat.ingestion.Models.FAC.Enums;

[Flags]
public enum ProviderType
{
    None = 0,
    FE = 1,
    NonLARS = 2,
    TLevels = 4
}