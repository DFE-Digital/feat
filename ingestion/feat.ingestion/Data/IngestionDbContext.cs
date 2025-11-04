using feat.common.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using FAC = feat.common.Models.Staging.FAC;
using FAA = feat.common.Models.Staging.FAA;

namespace feat.ingestion.Data;

public class IngestionDbContext(DbContextOptions<IngestionDbContext> options) : DbContext(options)
{
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlServer(b => b.MigrationsAssembly("feat.ingestion"));
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        var timeSpanToTicksConverter = new ValueConverter<TimeSpan?, long?>(
            v => v.HasValue ? v.Value.Ticks : null,
            v => v.HasValue ? TimeSpan.FromTicks(v.Value) : null
        );

        modelBuilder.Entity<EntryInstance>()
            .Property(e => e.Duration)
            .HasConversion(timeSpanToTicksConverter)
            .HasColumnType("bigint");
    }

    #region Our models
    
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
    
    #endregion
    
    #region Staging Models
    
    public DbSet<FAC.AllCoursesCourse> FAC_AllCourses { get; set; }
    public DbSet<FAC.Course> FAC_Courses { get; set; }
    public DbSet<FAC.TLevel> FAC_TLevels { get; set; }
    public DbSet<FAC.TLevelDefinition> FAC_TLevelDefinitions { get; set; }
    public DbSet<FAC.AimData> FAC_AimData { get; set; }
    
    public DbSet<FAA.Apprenticeship> FAA_Apprenticeships { get; set; }
    public DbSet<FAA.Address> FAA_Addresses { get; set; }
    
    #endregion
}