using Microsoft.EntityFrameworkCore;
using feat.common.Models;

namespace feat.api.Data;

public class CourseDbContext(DbContextOptions<CourseDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Create our composite keys
        modelBuilder.Entity<EntryLocation>()
            .HasKey(nameof(EntryLocation.EntryId), nameof(EntryLocation.LocationId));
        modelBuilder.Entity<EntrySector>()
            .HasKey(nameof(EntrySector.EntryId), nameof(EntrySector.SectorId));
        modelBuilder.Entity<ProviderLocation>()
            .HasKey(nameof(ProviderLocation.ProviderId), nameof(ProviderLocation.LocationId));
        modelBuilder.Entity<EmployerLocation>()
            .HasKey(e => new { e.EmployerId, e.LocationId });
    }
    
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
    
    public DbSet<PostcodeLatLong> Postcodes { get; set; }
}