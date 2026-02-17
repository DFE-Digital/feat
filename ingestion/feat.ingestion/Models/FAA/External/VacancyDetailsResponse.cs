namespace feat.ingestion.Models.FAA.External;

public class VacancyDetailsResponse
{
    public string? EmployerDescription { get; set; }
    
    public string? FullDescription { get; set; }
    
    public List<Qualification>? Qualifications { get; set; }
}
