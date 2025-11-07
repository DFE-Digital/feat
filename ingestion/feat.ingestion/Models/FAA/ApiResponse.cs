namespace feat.ingestion.Models.FAA;

public class ApiResponse
{
    public List<Apprenticeship> Vacancies { get; set; } = [];
    
    public int Total { get; set; }
    
    public int TotalFiltered { get; set; }
    
    public int TotalPages { get; set; }
}
