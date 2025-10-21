namespace feat.ingestion.Handlers;

public interface IMigrationsHandler
{
    bool RunPendingMigrations();
    
    bool HasPendingMigrations();
    bool HasPendingModelChanges();
}