namespace feat.ingestion.Models.External;

public class FindAnApprenticeshipApiResponse
{
    public List<ApprenticeshipCourse> Vacancies { get; set; } = [];
    public int Total { get; set; }
    public int TotalFiltered { get; set; }
    public int TotalPages { get; set; }
}
