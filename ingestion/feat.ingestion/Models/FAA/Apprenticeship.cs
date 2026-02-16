using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace feat.ingestion.Models.FAA;

[Table("FAA_Apprenticeships")]
public class Apprenticeship
{
    public object Wage;

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    [StringLength(100)]
    public string? Title { get; set; }
    
    public string? Description { get; set; }
    
    public string? FullDescription { get; set; }
    
    [StringLength(1000)]
    public string? QualificationsSummary { get; set; }
    
    public long NumberOfPositions { get; set; }
    
    public DateTime PostedDate { get; set; }
    
    public DateTime ClosingDate { get; set; }
    
    public DateTime? StartDate { get; set; }
    
    [StringLength(50)]
    public string WageType { get; set; }
    
    public double? WageAmount { get; set; }
    
    [StringLength(20)]
    public string WageUnit { get; set; }
    
    [StringLength(250)]
    public string? WageAdditionalInformation { get; set; }
    
    [StringLength(1000)]
    public string? WorkingWeekDescription { get; set; }
    
    public decimal? HoursPerWeek { get; set; }
    
    [StringLength(20)]
    public string? ExpectedDuration { get; set; }
    
    [StringLength(2048)]
    public string? ApplicationUrl { get; set; }
    
    public double? Distance { get; set; }
    
    [StringLength(100)]
    public string? EmployerName { get; set; }
    
    [StringLength(2048)]
    public string? EmployerWebsiteUrl { get; set; }
    
    [StringLength(100)]
    public string? EmployerContactName { get; set; }
    
    [StringLength(20)]
    public string? EmployerContactPhone { get; set; }
    
    [StringLength(254)]
    public string? EmployerContactEmail { get; set; }
    
    [StringLength(4000)]
    public string? EmployerDescription { get; set; }
    
    public int CourseLarsCode { get; set; }
    
    [StringLength(100)]
    public string? CourseTitle { get; set; }
    
    public int CourseLevel { get; set; }
    
    [StringLength(100)]
    public string? CourseRoute { get; set; }
    
    [StringLength(50)]
    public string? CourseType { get; set; }
    
    [StringLength(50)]
    public string? ApprenticeshipLevel { get; set; }
    
    [StringLength(100)]
    public string? ProviderName { get; set; }
    
    public int Ukprn { get; set; }
    
    public bool IsDisabilityConfident { get; set; }
    
    [StringLength(2048)]
    public string? VacancyUrl { get; set; }
    
    [StringLength(50)]
    public string? VacancyReference { get; set; }
    
    public bool IsNationalVacancy { get; set; }
    
    public DateTime? DetailsUpdated { get; set; }
    
    [StringLength(500)]
    public string? IsNationalVacancyDetails { get; set; }
    
    public ICollection<Address> Addresses { get; set; } = new List<Address>();
}