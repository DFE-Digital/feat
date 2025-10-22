namespace feat.ingestion.Models.FAA;

public class ApiResponse
{
    public List<ApprenticeshipCourse> Vacancies { get; set; } = [];
    
    public int Total { get; set; }
    
    public int TotalFiltered { get; set; }
    
    public int TotalPages { get; set; }
}
