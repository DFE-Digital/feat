namespace feat.ingestion.Models.FAA.External;

public class Apprenticeship
{
    public string? Title { get; set; }
    
    public string? Description { get; set; }
    
    public long NumberOfPositions { get; set; }
    
    public DateTime PostedDate { get; set; }
    
    public DateTime ClosingDate { get; set; }
    
    public DateTime StartDate { get; set; }
    
    public required Wage Wage { get; set; }
    
    public decimal? HoursPerWeek { get; set; }
    
    public string? ExpectedDuration { get; set; }
    
    public List<Address>? Addresses { get; set; }
    
    public string? ApplicationUrl { get; set; }
    
    public double? Distance { get; set; }
    
    public string? EmployerName { get; set; }
    
    public string? EmployerWebsiteUrl { get; set; }
    
    public string? EmployerContactName { get; set; }
    
    public string? EmployerContactPhone { get; set; }
    
    public string? EmployerContactEmail { get; set; }
    
    public required Course Course { get; set; }
    
    public string? ApprenticeshipLevel { get; set; }
    
    public string? ProviderName { get; set; }
    
    public int Ukprn { get; set; }
    
    public bool IsDisabilityConfident { get; set; }
    
    public string? VacancyUrl { get; set; }
    
    public string? VacancyReference { get; set; }
    
    public bool IsNationalVacancy { get; set; }
    
    public string? IsNationalVacancyDetails { get; set; }
}