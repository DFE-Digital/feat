using feat.common;
using feat.common.Models;
using feat.ingestion.Data;
using feat.ingestion.Models.External;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace feat.ingestion.Services;

public class FindAnApprenticeshipIngestionService : IIngestionService
{
    private readonly IApiClient _apiClient;
    private readonly IngestionDbContext _dbContext;
    private readonly ILogger<FindAnApprenticeshipIngestionService> _logger;
    private readonly AsyncRetryPolicy _retryPolicy;
    
    public FindAnApprenticeshipIngestionService(
        IApiClient apiClient,
        IngestionDbContext dbContext,
        ILogger<FindAnApprenticeshipIngestionService> logger)
    {
        _apiClient = apiClient;
        _dbContext = dbContext;
        _logger = logger;
        
        _retryPolicy = Policy
            .Handle<HttpRequestException>()
            .Or<TaskCanceledException>()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                onRetry: (exception, timeSpan, retryCount) =>
                {
                    _logger.LogWarning(
                        "Retry {RetryCount} after {Delay}s due to error: {Message}",
                        retryCount, timeSpan.TotalSeconds, exception.Message);
                });
    }

    public async Task IngestAsync(CancellationToken cancellationToken = default)
    {
        const int pageSize = 500;
        
        _logger.LogInformation("Starting Find An Apprenticeship ingestion...");
        
        try
        {
            var allCourses = new List<ApprenticeshipCourse>();
            var pageNumber = 1;

            while (true)
            {
                _logger.LogInformation("Fetching page {PageNumber} (PageSize={PageSize}).", pageNumber, pageSize);
                
                var url = $"vacancies/vacancy?PageNumber={pageNumber}&PageSize={pageSize}";

                var response = await _retryPolicy.ExecuteAsync(async () =>
                    await _apiClient.GetAsync<FindAnApprenticeshipApiResponse>(
                        ApiClientNames.FindAnApprenticeship, url, cancellationToken: cancellationToken)
                );
                
                var courses = response?.Vacancies ?? [];

                if (courses.Count == 0)
                {
                    _logger.LogInformation("No more courses found after page {PageNumber}. Stopping pagination.", pageNumber);
                    break;
                }

                allCourses.AddRange(courses);
                
                _logger.LogInformation("Fetched {Count} courses from page {PageNumber}.", courses.Count, pageNumber);

                if (courses.Count < pageSize)
                {
                    _logger.LogInformation("Last page detected (page {PageNumber}).", pageNumber);
                    break;
                }

                pageNumber++;
            }

            if (allCourses.Count == 0)
            {
                _logger.LogWarning("No apprenticeship data retrieved from the API.");
                return;
            }

            var stagingRows = allCourses.Select(c => new StagingApprenticeshipCourse
            {
                Title = c.Title,
                Description = c.Description,
                NumberOfPositions = c.NumberOfPositions,
                PostedDate = c.PostedDate,
                ClosingDate = c.ClosingDate,
                StartDate = c.StartDate,
                WageType = c.Wage.WageType,
                WageAmount = c.Wage.WageAmount,
                WageUnit = c.Wage.WageUnit,
                WageAdditionalInformation = c.Wage.WageAdditionalInformation,
                WorkingWeekDescription = c.Wage.WorkingWeekDescription,
                HoursPerWeek = c.HoursPerWeek,
                ExpectedDuration = c.ExpectedDuration,
                // Only get first address from list for now
                AddressLine1 = c.Addresses.FirstOrDefault()?.AddressLine1,
                AddressLine2 = c.Addresses.FirstOrDefault()?.AddressLine2,
                AddressLine3 = c.Addresses.FirstOrDefault()?.AddressLine3,
                AddressLine4 = c.Addresses.FirstOrDefault()?.AddressLine4,
                AddressPostcode = c.Addresses.FirstOrDefault()?.Postcode,
                AddressLatitude = c.Addresses.FirstOrDefault()?.Latitude,
                AddressLongitude = c.Addresses.FirstOrDefault()?.Longitude,
                ApplicationUrl = c.ApplicationUrl,
                Distance = c.Distance,
                EmployerName = c.EmployerName,
                EmployerWebsiteUrl = c.EmployerWebsiteUrl,
                EmployerContactName = c.EmployerContactName,
                EmployerContactPhone = c.EmployerContactPhone,
                EmployerContactEmail = c.EmployerContactEmail,
                CourseLarsCode = c.Course.LarsCode,
                CourseTitle = c.Course.Title,
                CourseLevel = c.Course.Level,
                CourseRoute = c.Course.Route,
                CourseType = c.Course.Type,
                ApprenticeshipLevel = c.ApprenticeshipLevel,
                ProviderName = c.ProviderName,
                Ukprn = c.Ukprn,
                IsDisabilityConfident = c.IsDisabilityConfident,
                VacancyUrl = c.VacancyUrl,
                VacancyReference = c.VacancyReference,
                IsNationalVacancy = c.IsNationalVacancy,
                IsNationalVacancyDetails = c.IsNationalVacancyDetails
            }).ToList();

            await _dbContext.StagingApprenticeshipCourses.AddRangeAsync(stagingRows, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Successfully ingested {Count} courses into staging.", stagingRows.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to ingest Find An Apprenticeship data.");
        }
    }
}