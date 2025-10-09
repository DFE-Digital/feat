using Microsoft.EntityFrameworkCore;
using feat.common.Models;

namespace feat.api.Data;

public class IngestionDbContext : DbContext
{
    public DbSet<Employer> Employers { get; set; }
    public DbSet<EmployerLocation> EmployerLocations { get; set; }
    public DbSet<Entry> Entries {get; set;}
    public DbSet<EntryCost> EntryCosts { get; set; }
    public DbSet<EntryInstance> EntryInstances { get; set; }
    public DbSet<EntryLocation> EntryLocations { get; set; }
    public DbSet<EntrySector> EntrySectors { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<Provider> Providers { get; set; }
    public DbSet<ProviderLocation> ProviderLocations { get; set; }
    public DbSet<Sector> Sectors { get; set; }
    public DbSet<UniversityCourse> UniversitiesCourses { get; set; }
    public DbSet<Vacancy> Vacancies { get; set; }

    public IngestionDbContext(DbContextOptions<IngestionDbContext> options): base(options)
    {   
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
    }
}