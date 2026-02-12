namespace feat.ingestion.Models.FAA.External;

public class VacancyResponse
{
    public List<Apprenticeship> Vacancies { get; set; } = [];
    
    public int Total { get; set; }
    
    public int TotalFiltered { get; set; }
    
    public int TotalPages { get; set; }
}
