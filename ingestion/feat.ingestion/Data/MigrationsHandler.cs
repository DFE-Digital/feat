using Microsoft.EntityFrameworkCore;

namespace feat.ingestion.Data;

public interface IMigrationsHandler
{
    bool RunPendingMigrations();
}

public class MigrationsHandler : IMigrationsHandler
{
    private readonly IngestionDbContext _dbContext;
    
    public MigrationsHandler(IngestionDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public bool RunPendingMigrations()
    {
        try
        {
            var pendingMigrations = _dbContext.Database.GetPendingMigrations();
            if (pendingMigrations.Any())
            {
                Console.WriteLine("FEAT ingestion migrations found.");
                _dbContext.Database.Migrate();

                Console.WriteLine("FEAT ingestion migrations applied.");
            }
            else
            {
                Console.WriteLine("FEAT ingestion no migrations found.");
            }
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine($"FEAT ingestion development migration Exception: {e}");
        }
        return false;
    }
}
