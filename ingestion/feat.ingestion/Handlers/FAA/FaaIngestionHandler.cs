using feat.common;
using feat.ingestion.Configuration;
using feat.ingestion.Data;
using feat.ingestion.Enums;
using feat.ingestion.Models.FAA;
using Microsoft.EntityFrameworkCore;
using Database = feat.common.Models.Staging.FAA;

namespace feat.ingestion.Handlers.FAA;

public class FaaIngestionHandler(
    IngestionOptions options,
    IApiClient apiClient,
    IngestionDbContext dbContext,
    ISearchIndexHandler searchIndexHandler)
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
                
                var courses = response.Vacancies;

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
            
            var rowsToTransform = new List<(Database.Apprenticeship Row, bool HasChanges)>();

            foreach (var course in allCourses)
            {
                var existing = await dbContext.FAA_Apprenticeships
                    .Include(x => x.Addresses)
                    .FirstOrDefaultAsync(a => a.VacancyReference == course.VacancyReference, cancellationToken);

                if (existing == null)
                {
                    // New record
                    var staged = new Database.Apprenticeship
                    {
                        Title = course.Title,
                        Description = course.Description,
                        NumberOfPositions = course.NumberOfPositions,
                        PostedDate = course.PostedDate,
                        ClosingDate = course.ClosingDate,
                        StartDate = course.StartDate,
                        WageType = course.Wage.WageType,
                        WageAmount = course.Wage.WageAmount,
                        WageUnit = course.Wage.WageUnit,
                        WageAdditionalInformation = course.Wage.WageAdditionalInformation,
                        WorkingWeekDescription = course.Wage.WorkingWeekDescription,
                        HoursPerWeek = course.HoursPerWeek,
                        ExpectedDuration = course.ExpectedDuration,
                        ApplicationUrl = course.ApplicationUrl,
                        Distance = course.Distance,
                        EmployerName = course.EmployerName,
                        EmployerWebsiteUrl = course.EmployerWebsiteUrl,
                        EmployerContactName = course.EmployerContactName,
                        EmployerContactPhone = course.EmployerContactPhone,
                        EmployerContactEmail = course.EmployerContactEmail,
                        CourseLarsCode = course.Course.LarsCode,
                        CourseTitle = course.Course.Title,
                        CourseLevel = course.Course.Level,
                        CourseRoute = course.Course.Route,
                        CourseType = course.Course.Type,
                        ApprenticeshipLevel = course.ApprenticeshipLevel,
                        ProviderName = course.ProviderName,
                        Ukprn = course.Ukprn,
                        IsDisabilityConfident = course.IsDisabilityConfident,
                        VacancyUrl = course.VacancyUrl,
                        VacancyReference = course.VacancyReference,
                        IsNationalVacancy = course.IsNationalVacancy,
                        IsNationalVacancyDetails = course.IsNationalVacancyDetails,
                        Addresses = course.Addresses?.Select(add => new Database.Address
                        {
                            AddressLine1 = add.AddressLine1,
                            AddressLine2 = add.AddressLine2,
                            AddressLine3 = add.AddressLine3,
                            AddressLine4 = add.AddressLine4,
                            Postcode = add.Postcode,
                            Latitude = add.Latitude,
                            Longitude = add.Longitude
                        }).ToList() ?? []
                    };

                    await dbContext.FAA_Apprenticeships.AddAsync(staged, cancellationToken);
                    rowsToTransform.Add((staged, false));
                }
                else
                {
                    // Update if changed
                    var hasChanges = false;

                    if (existing.Title != course.Title) { existing.Title = course.Title; hasChanges = true; }
                    if (existing.Description != course.Description) { existing.Description = course.Description; hasChanges = true; }
                    if (existing.NumberOfPositions != course.NumberOfPositions) { existing.NumberOfPositions = course.NumberOfPositions; hasChanges = true; }
                    if (existing.PostedDate != course.PostedDate) { existing.PostedDate = course.PostedDate; hasChanges = true; }
                    if (existing.ClosingDate != course.ClosingDate) { existing.ClosingDate = course.ClosingDate; hasChanges = true; }
                    if (existing.StartDate != course.StartDate) { existing.StartDate = course.StartDate; hasChanges = true; }
                    if (existing.WageType != course.Wage.WageType) { existing.WageType = course.Wage.WageType; hasChanges = true; }
                    if (!existing.WageAmount.Equals(course.Wage.WageAmount)) { existing.WageAmount = course.Wage.WageAmount; hasChanges = true; }
                    if (existing.WageUnit != course.Wage.WageUnit) { existing.WageUnit = course.Wage.WageUnit; hasChanges = true; }
                    if (existing.WageAdditionalInformation != course.Wage.WageAdditionalInformation) { existing.WageAdditionalInformation = course.Wage.WageAdditionalInformation; hasChanges = true; }
                    if (existing.WorkingWeekDescription != course.Wage.WorkingWeekDescription) { existing.WorkingWeekDescription = course.Wage.WorkingWeekDescription; hasChanges = true; }
                    if (!existing.HoursPerWeek.Equals(course.HoursPerWeek)) { existing.HoursPerWeek = course.HoursPerWeek; hasChanges = true; }
                    if (existing.ExpectedDuration != course.ExpectedDuration) { existing.ExpectedDuration = course.ExpectedDuration; hasChanges = true; }
                    if (existing.ApplicationUrl != course.ApplicationUrl) { existing.ApplicationUrl = course.ApplicationUrl; hasChanges = true; }
                    if (!existing.Distance.Equals(course.Distance)) { existing.Distance = course.Distance; hasChanges = true; }
                    if (existing.EmployerName != course.EmployerName) { existing.EmployerName = course.EmployerName; hasChanges = true; }
                    if (existing.EmployerWebsiteUrl != course.EmployerWebsiteUrl) { existing.EmployerWebsiteUrl = course.EmployerWebsiteUrl; hasChanges = true; }
                    if (existing.EmployerContactName != course.EmployerContactName) { existing.EmployerContactName = course.EmployerContactName; hasChanges = true; }
                    if (existing.EmployerContactPhone != course.EmployerContactPhone) { existing.EmployerContactPhone = course.EmployerContactPhone; hasChanges = true; }
                    if (existing.EmployerContactEmail != course.EmployerContactEmail) { existing.EmployerContactEmail = course.EmployerContactEmail; hasChanges = true; }
                    if (existing.CourseLarsCode != course.Course.LarsCode) { existing.CourseLarsCode = course.Course.LarsCode; hasChanges = true; }
                    if (existing.CourseTitle != course.Course.Title) { existing.CourseTitle = course.Course.Title; hasChanges = true; }
                    if (existing.CourseLevel != course.Course.Level) { existing.CourseLevel = course.Course.Level; hasChanges = true; }
                    if (existing.CourseRoute != course.Course.Route) { existing.CourseRoute = course.Course.Route; hasChanges = true; }
                    if (existing.CourseType != course.Course.Type) { existing.CourseType = course.Course.Type; hasChanges = true; }
                    if (existing.ApprenticeshipLevel != course.ApprenticeshipLevel) { existing.ApprenticeshipLevel = course.ApprenticeshipLevel; hasChanges = true; }
                    if (existing.ProviderName != course.ProviderName) { existing.ProviderName = course.ProviderName; hasChanges = true; }
                    if (existing.Ukprn != course.Ukprn) { existing.Ukprn = course.Ukprn; hasChanges = true; }
                    if (existing.IsDisabilityConfident != course.IsDisabilityConfident) { existing.IsDisabilityConfident = course.IsDisabilityConfident; hasChanges = true; }
                    if (existing.VacancyUrl != course.VacancyUrl) { existing.VacancyUrl = course.VacancyUrl; hasChanges = true; }
                    if (existing.IsNationalVacancy != course.IsNationalVacancy) { existing.IsNationalVacancy = course.IsNationalVacancy; hasChanges = true; }
                    if (existing.IsNationalVacancyDetails != course.IsNationalVacancyDetails) { existing.IsNationalVacancyDetails = course.IsNationalVacancyDetails; hasChanges = true; }
                    
                    foreach (var address in course.Addresses)
                    {
                        if (!existing.Addresses.Any(a => a.Postcode == address.Postcode && a.AddressLine1 == address.AddressLine1))
                        {
                            existing.Addresses.Add(new Database.Address
                            {
                                AddressLine1 = address.AddressLine1,
                                AddressLine2 = address.AddressLine2,
                                AddressLine3 = address.AddressLine3,
                                AddressLine4 = address.AddressLine4,
                                Postcode = address.Postcode,
                                Latitude = address.Latitude,
                                Longitude = address.Longitude
                            });
                            
                            hasChanges = true;
                        }
                    }

                    if (hasChanges)
                    {
                        rowsToTransform.Add((existing, true));
                    }
                }
            }

            await dbContext.SaveChangesAsync(cancellationToken);
            Console.WriteLine($"Ingested {rowsToTransform.Count} new or updated Find An Apprenticeship records into staging.");
            
            if (rowsToTransform.Count == 0)
            {
                Console.WriteLine("No staged FAA records to transform.");
            }
            else
            {
                var transformer = new FaaTransformHandler(dbContext);
                var aiSearchEntries = await transformer.TransformAsync(rowsToTransform, cancellationToken);

                foreach (var aiSearchEntry in aiSearchEntries)
                {
                    aiSearchEntry.TitleVector = searchIndexHandler.GetVector(aiSearchEntry.Title);
                    aiSearchEntry.DescriptionVector = searchIndexHandler.GetVector(aiSearchEntry.Description);
                    aiSearchEntry.LearningAimTitleVector = searchIndexHandler.GetVector(aiSearchEntry.LearningAimTitle);
                    aiSearchEntry.SectorVector = searchIndexHandler.GetVector(aiSearchEntry.Sector);
                }
        
                var success = searchIndexHandler.Ingest(aiSearchEntries);
                Console.WriteLine($"FAA entries ingested to AI Search: {success}");
            }

            Console.WriteLine($"Find An Apprenticeship ingestion complete.");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to ingest Find An Apprenticeship data: {ex.Message}");
            return false;
        }
    }
}
