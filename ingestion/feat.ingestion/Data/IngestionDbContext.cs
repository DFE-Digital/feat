using feat.common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using Microsoft.Extensions.Configuration;


namespace feat.ingestion.Data;

public class IngestionDbContext : DbContext
{
    public DbSet<Employer> Employers { get; set; }
    
    public DbSet<EmployerLocation> EmployerLocations { get; set; }

    public DbSet<Entry> Entries { get; set; }

    public DbSet<EntryCost> EntryCosts { get; set; }

    public DbSet<EntryInstance> EntryInstances { get; set; }

    public DbSet<EntryLocation> EntryLocations { get; set; }

    public DbSet<EntrySector> EntrySectors { get; set; }

    public DbSet<Location> Locations { get; set; }

    public DbSet<Provider> Providers { get; set; }

    public DbSet<ProviderLocation> ProviderLocations { get; set; }

    public DbSet<Sector> Sectors { get; set; }

    public DbSet<UniversityCourse> UniversityCourses { get; set; }

    public DbSet<Vacancy> Vacancies { get; set; }
    
    private readonly string _connectionString = String.Empty;

    public IngestionDbContext(string connection)
    {
        _connectionString = connection;
    }

    public IngestionDbContext(DbContextOptions<IngestionDbContext> options)
        : base(options)
    {
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(_connectionString, x=> x.UseNetTopologySuite());
        }
    }
}