using Microsoft.EntityFrameworkCore;

namespace feat.ingestion.Data;

public class MigrationsHandler(IngestionDbContext dbContext) : IMigrationsHandler
{
    public bool RunPendingMigrations()
    {
        try
        {
            var pendingMigrations = dbContext.Database.GetPendingMigrations();
            if (pendingMigrations.Any())
            {
                Console.WriteLine("FEAT ingestion migrations found.");
                dbContext.Database.Migrate();

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
            throw;
        }
        return false;
    }

    public bool HasPendingMigrations()
    {
        try
        {
            var pendingMigrations = dbContext.Database.GetPendingMigrations();
            return pendingMigrations.Any();
        }
        catch (Exception e)
        {
            Console.WriteLine($"FEAT ingestion development migration Exception: {e}");
            throw;
        }

        return false;
    }

    public bool HasPendingModelChanges()
    {
        try
        {
            var pendingModelChanges = dbContext.Database.HasPendingModelChanges();
            return pendingModelChanges;
        }
        catch (Exception e)
        {
            Console.WriteLine($"FEAT ingestion development migration Exception: {e}");
            throw;
        }

        return false;    }
}
