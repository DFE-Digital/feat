namespace feat.ingestion.Enums;

[Flags]
public enum IngestionType
{
    Manual = 0,
    Automatic = 1,
    Api = 2,
    Csv = 4,
    Json = 8,
    Xml = 16,
    Excel = 32,
    Database = 64,
    Compressed = 128
}