namespace feat.ingestion.Data;

public interface IMigrationsHandler
{
    bool RunPendingMigrations();
    
    bool HasPendingMigrations();
    bool HasPendingModelChanges();
}