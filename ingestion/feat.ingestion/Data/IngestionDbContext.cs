using feat.common.Models;
using Microsoft.EntityFrameworkCore;
using FAC = feat.ingestion.Models.FAC;
using FAA = feat.ingestion.Models.FAA;
using DU = feat.ingestion.Models.DU;

namespace feat.ingestion.Data;

public class IngestionDbContext(DbContextOptions<IngestionDbContext> options) : DbContext(options)
{
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlServer(b => b.MigrationsAssembly("feat.ingestion"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Create our composite keys
        modelBuilder.Entity<EntrySector>()
            .HasKey(nameof(EntrySector.EntryId), nameof(EntrySector.SectorId));
        modelBuilder.Entity<ProviderLocation>()
            .HasKey(nameof(ProviderLocation.ProviderId), nameof(ProviderLocation.LocationId));
        modelBuilder.Entity<EmployerLocation>()
            .HasKey(nameof(EmployerLocation.EmployerId), nameof(EmployerLocation.LocationId));
        
        // Composite keys for Discover Uni
        modelBuilder.Entity<DU.Location>()
            .HasKey(nameof(DU.Location.UKPRN), nameof(DU.Location.LocationId));
        modelBuilder.Entity<DU.Course>()
            .HasKey(nameof(DU.Course.UKPRN), nameof(DU.Course.CourseId), nameof(DU.Course.StudyMode));
        modelBuilder.Entity<DU.CourseLocation>()
            .HasKey(nameof(DU.CourseLocation.UKPRN), nameof(DU.Institution.PubUKPRN), nameof(DU.CourseLocation.CourseId), 
                nameof(DU.Course.StudyMode), nameof(DU.CourseLocation.LocationId));
        modelBuilder.Entity<DU.Institution>()
            .HasKey(nameof(DU.Institution.UKPRN), nameof(DU.Institution.PubUKPRN));
        
    }

    #region Our models
    
    public DbSet<Employer> Employers { get; set; }
    
    public DbSet<EmployerLocation> EmployerLocations { get; set; }

    public DbSet<Entry> Entries { get; set; }

    public DbSet<EntryCost> EntryCosts { get; set; }

    public DbSet<EntryInstance> EntryInstances { get; set; }

    public DbSet<EntrySector> EntrySectors { get; set; }

    public DbSet<Location> Locations { get; set; }

    public DbSet<Provider> Providers { get; set; }

    public DbSet<ProviderLocation> ProviderLocations { get; set; }

    public DbSet<Sector> Sectors { get; set; }

    public DbSet<UniversityCourse> UniversityCourses { get; set; }

    public DbSet<Vacancy> Vacancies { get; set; }
    
    public DbSet<PostcodeLatLong> Postcodes { get; set; }
    
    #endregion
    
    #region Staging Models
    
    #region Find a Course / Publish to Course Directory
    public DbSet<FAC.AllCoursesCourse> FAC_AllCourses { get; set; }
    public DbSet<FAC.Course> FAC_Courses { get; set; }
    public DbSet<FAC.CourseRun> FAC_CourseRuns { get; set; }
    public DbSet<FAC.TLevel> FAC_TLevels { get; set; }
    public DbSet<FAC.TLevelDefinition> FAC_TLevelDefinitions { get; set; }
    public DbSet<FAC.TLevelLocation> FAC_TLevelLocations { get; set; }
    public DbSet<FAC.AimData> FAC_AimData { get; set; }
    public DbSet<FAC.ApprovedQualification> FAC_ApprovedQualifications { get; set; }
    public DbSet<FAC.Provider> FAC_Providers { get; set; }
    public DbSet<FAC.Venue> FAC_Venues { get; set; }
    
    #endregion
    
    #region Find an Apprenticeship
    
    public DbSet<FAA.Apprenticeship> FAA_Apprenticeships { get; set; }
    public DbSet<FAA.Address> FAA_Addresses { get; set; }
    
    #endregion
    
    #region Discover Uni
    
    public DbSet<DU.IngestionState> DU_IngestionState { get; set; }
    public DbSet<DU.Location> DU_Locations { get; set; }
    public DbSet<DU.Institution> DU_Institutions { get; set; }
    public DbSet<DU.Aim> DU_Aims { get; set; }
    public DbSet<DU.Hecos> DU_HECOS { get; set; }
    public DbSet<DU.Course> DU_Courses { get; set; }
    public DbSet<DU.CourseLocation> DU_CourseLocations { get; set; }
    
    #endregion
    
    #endregion
}