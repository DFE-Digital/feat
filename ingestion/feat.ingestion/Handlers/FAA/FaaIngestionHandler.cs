using feat.common;
using feat.ingestion.Configuration;
using feat.ingestion.Data;
using feat.ingestion.Enums;
using feat.ingestion.Models.FAA;
using Database = feat.common.Models.Staging.FAA;

namespace feat.ingestion.Handlers.FAA;

public class FaaIngestionHandler(
    IngestionOptions options,
    IApiClient apiClient,
    IngestionDbContext dbContext)
    : IngestionHandler(options)
{
    private readonly IngestionOptions _options = options;

    public override IngestionType IngestionType => IngestionType.Api | IngestionType.Automatic;
    public override string Name => "Find An Apprenticeship";
    public override string Description => "Ingestion from Find An Apprenticeship API";
    
    public override async Task<bool> ValidateAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(_options.ApprenticeshipApiKey))
        {
            Console.WriteLine("Find An Apprenticeship API key not set.");
            return false;
        }
        
        const string url = "vacancies/vacancy?PageNumber=1&PageSize=1";
        ApiResponse response;

        try
        {
            response = await apiClient.GetAsync<ApiResponse>(
                ApiClientNames.FindAnApprenticeship, url, cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return false;
        }

        if (response.Total == 0)
        {
            Console.WriteLine("No results returned from Find An Apprenticeship API.");
            return false;
        }
        
        return true;
    }

    public override async Task<bool> IngestAsync(CancellationToken cancellationToken)
    {
        const int pageSize = 500;
        
        try
        {
            var allCourses = new List<ApprenticeshipCourse>();
            var pageNumber = 1;

            while (true)
            {
                Console.WriteLine($"Fetching page {pageNumber} (PageSize={pageSize})...");
                
                var url = $"vacancies/vacancy?PageNumber={pageNumber}&PageSize={pageSize}";

                var response = await apiClient.GetAsync<ApiResponse>(
                    ApiClientNames.FindAnApprenticeship, url, cancellationToken: cancellationToken);
                
                var courses = response?.Vacancies ?? [];

                if (courses.Count == 0)
                {
                    Console.WriteLine($"No more courses found after page {pageNumber}. Stopping pagination.");
                    break;
                }

                allCourses.AddRange(courses);
                
                Console.WriteLine($"Fetched {courses.Count} courses from page {pageNumber}.");

                if (courses.Count < pageSize)
                {
                    Console.WriteLine($"Last page detected (page {pageNumber}).");
                    break;
                }

                pageNumber++;
            }

            if (allCourses.Count == 0)
            {
                Console.WriteLine("No apprenticeship data retrieved from the API.");
                return false;
            }

            var stagingRows = allCourses.Select(app => new Database.Apprenticeship
            {
                Title = app.Title,
                Description = app.Description,
                NumberOfPositions = app.NumberOfPositions,
                PostedDate = app.PostedDate,
                ClosingDate = app.ClosingDate,
                StartDate = app.StartDate,
                WageType = app.Wage.WageType,
                WageAmount = app.Wage.WageAmount,
                WageUnit = app.Wage.WageUnit,
                WageAdditionalInformation = app.Wage.WageAdditionalInformation,
                WorkingWeekDescription = app.Wage.WorkingWeekDescription,
                HoursPerWeek = app.HoursPerWeek,
                ExpectedDuration = app.ExpectedDuration,
                ApplicationUrl = app.ApplicationUrl,
                Distance = app.Distance,
                EmployerName = app.EmployerName,
                EmployerWebsiteUrl = app.EmployerWebsiteUrl,
                EmployerContactName = app.EmployerContactName,
                EmployerContactPhone = app.EmployerContactPhone,
                EmployerContactEmail = app.EmployerContactEmail,
                CourseLarsCode = app.Course.LarsCode,
                CourseTitle = app.Course.Title,
                CourseLevel = app.Course.Level,
                CourseRoute = app.Course.Route,
                CourseType = app.Course.Type,
                ApprenticeshipLevel = app.ApprenticeshipLevel,
                ProviderName = app.ProviderName,
                Ukprn = app.Ukprn,
                IsDisabilityConfident = app.IsDisabilityConfident,
                VacancyUrl = app.VacancyUrl,
                VacancyReference = app.VacancyReference,
                IsNationalVacancy = app.IsNationalVacancy,
                IsNationalVacancyDetails = app.IsNationalVacancyDetails,
                Addresses = app.Addresses?.Select(add => new Database.Address
                {
                    AddressLine1 = add.AddressLine1,
                    AddressLine2 = add.AddressLine2,
                    AddressLine3 = add.AddressLine3,
                    AddressLine4 = add.AddressLine4,
                    Postcode = add.Postcode,
                    Latitude = add.Latitude,
                    Longitude = add.Longitude
                }).ToList() ?? []
            }).ToList();

            await dbContext.FAA_Apprenticeships.AddRangeAsync(stagingRows, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            Console.WriteLine($"Successfully ingested {stagingRows.Count} courses into staging.");
            return true;

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to ingest Find An Apprenticeship data: {ex.Message}");
            return false;
        }
    }
}
