using System.Globalization;
using Azure.Search.Documents;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using CsvHelper;
using EnumsNET;
using feat.common.Models;
using feat.common.Models.AiSearch;
using feat.common.Models.Enums;
using feat.common.Models.Staging.FAC;
using feat.common.Models.Staging.FAC.Enums;
using feat.ingestion.Configuration;
using feat.ingestion.Data;
using feat.ingestion.Enums;
using Microsoft.Extensions.Options;
using Microsoft.Spatial;
using OpenAI.Embeddings;

namespace feat.ingestion.Handlers.FAC;

public class FacIngestionHandler(
    IngestionOptions options, 
    IngestionDbContext dbContext,
    ISearchIndexHandler searchIndexHandler) 
    : IngestionHandler(options)
{
    public override IngestionType IngestionType => IngestionType.Csv | IngestionType.Manual;
    public override string Name => "Find A Course";
    public override string Description => "File based ingestion from Find A Course, Publish To Course Directory, and Learning AIM Datasets Manually Uploaded into Blob Storage";

    private const string ContainerName = "fac";
    
    
    public override async Task<bool> ValidateAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(options.BlobStorageConnectionString))
        {
            Console.WriteLine("Blob storage connection string not set");
            return false;
        }

        var blobServiceClient = new BlobServiceClient(options.BlobStorageConnectionString);
        var containerClient = blobServiceClient.GetBlobContainerClient(ContainerName);
        var exists = await containerClient.ExistsAsync(cancellationToken);
        if (!exists)
        {
            Console.WriteLine("Storage container doesn't exist, attempting to create");
        }

        await containerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);
        exists = await containerClient.ExistsAsync(cancellationToken);

        // Check the container exists
        if (!exists)
        {
            Console.WriteLine("Unable create the FAC Azure Storage Container");
            return false;
        }

        // Now check we have some files
        var files = containerClient.GetBlobsAsync(cancellationToken: cancellationToken);

        var foundAllCourses = false;
        var foundCourses = false;
        var foundTLevels = false;
        var foundAIMData = false;

        await foreach (var blob in files)
        {
            if (blob.Name.Contains("Courses_", StringComparison.InvariantCultureIgnoreCase))
            {
                foundCourses = true;
            }

            if (blob.Name.Contains("TLevels_", StringComparison.InvariantCultureIgnoreCase))
            {
                foundTLevels = true;
            }

            if (blob.Name.Contains("AllCourses", StringComparison.InvariantCultureIgnoreCase))
            {
                foundAllCourses = true;
            }

            if (blob.Name.Contains("LearningDelivery", StringComparison.InvariantCultureIgnoreCase))
            {
                foundAIMData = true;
            }
        }

        if (!foundAllCourses)
        {
            Console.WriteLine("Unable to find all courses data file");
            return false;
        }

        if (!foundCourses)
        {
            Console.WriteLine("Unable to find course text data file");
            return false;
        }

        if (!foundTLevels)
        {
            Console.WriteLine("Unable to find t-level text data file");
            return false;
        }

        if (!foundAIMData)
        {
            Console.WriteLine("Unable to find AIM data file");
            return false;
        }

        // Otherwise, we're returning true
        return true;

    }

    public override async Task<bool> IngestAsync(CancellationToken cancellationToken)
    {
        ProcessMode Aim = ProcessMode.Skip;
        ProcessMode Courses = ProcessMode.Skip;
        ProcessMode TLevels = ProcessMode.Skip;
        ProcessMode AllCourses = ProcessMode.Skip;
        int batchSize = 5000;

        var blobServiceClient = new BlobServiceClient(options.BlobStorageConnectionString);
        var containerClient = blobServiceClient.GetBlobContainerClient(ContainerName);
        await containerClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken);
        var valid = await ValidateAsync(cancellationToken);

        // If we're not passing validation, stop
        if (!valid)
        {
            Console.WriteLine("Unable to validate course data, stopping");
            return false;
        }

        // Let's stream our data in
        var files = containerClient.GetBlobsAsync(cancellationToken: cancellationToken)
            .ToBlockingEnumerable(cancellationToken).ToArray();

        // Get our latest AIM Data file
        var aimData = files.Where(blob =>
                blob.Name.Contains("LearningDelivery", StringComparison.InvariantCultureIgnoreCase))
            .OrderByDescending(b => b.Properties.CreatedOn).LastOrDefault();
        if (aimData != null && Aim != ProcessMode.Skip)
        {
            Console.WriteLine("Starting import of AIM Data");
            var blobClient = containerClient.GetBlobClient(aimData.Name);
            Console.WriteLine("Fetching file");
            using var reader = new StreamReader(await blobClient.OpenReadAsync(cancellationToken: cancellationToken));
            Console.WriteLine("Setting up CSV reader");
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            csv.Context.RegisterClassMap<AimDataMap>();
            Console.WriteLine("Reading data...");
            var records = csv.GetRecords<AimData>().ToList();

            if (
                Aim == ProcessMode.Force
                || dbContext.FAC_AimData.Count() != records.Count
            )
            {
                Console.WriteLine($"Preparing {records.Count} records to DB...");
                await dbContext.BulkSynchronizeAsync(records, options =>
                {
                    options.BatchSize = batchSize;
                    options.BatchDelayInterval = 1000;
                    options.UseTableLock = true;
                }, cancellationToken);
            }

            Console.WriteLine("Done");
        }

        // Get our latest courses file
        var courseData = files.Where(blob =>
                blob.Name.Contains("Courses_", StringComparison.InvariantCultureIgnoreCase))
            .OrderByDescending(b => b.Properties.CreatedOn).LastOrDefault();
        if (courseData != null && Courses != ProcessMode.Skip)
        {
            Console.WriteLine("Starting import of Course Data");
            var blobClient = containerClient.GetBlobClient(courseData.Name);
            Console.WriteLine("Fetching file");
            using var reader = new StreamReader(await blobClient.OpenReadAsync(cancellationToken: cancellationToken));
            Console.WriteLine("Setting up CSV reader");
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            csv.Context.RegisterClassMap<CourseMap>();
            Console.WriteLine("Reading data...");
            var records = csv.GetRecords<Course>().ToList();

            // Determine if there are any changes
            if (records.Count != 0)
            {
                var lastUpdated = records.Max(x => x.UpdatedOn);
                var lastCreated = records.Max(x => x.CreatedOn);

                if (
                    Courses == ProcessMode.Force
                    || dbContext.FAC_Courses.Count() != records.Count
                    || dbContext.FAC_Courses.Max(x => x.UpdatedOn) < lastUpdated
                    || dbContext.FAC_Courses.Max(x => x.CreatedOn) < lastCreated
                )
                {
                    Console.WriteLine($"Preparing {records.Count} records to DB...");
                    await dbContext.BulkSynchronizeAsync(records, options =>
                    {
                        options.BatchSize = batchSize;
                        options.BatchDelayInterval = 1000;
                        options.UseTableLock = true;
                    }, cancellationToken);
                }
            }

            Console.WriteLine("Done");
        }

        // Get our latest T Levels file
        var tLevelData = files.Where(blob =>
                blob.Name.Contains("TLevels_", StringComparison.InvariantCultureIgnoreCase))
            .OrderByDescending(b => b.Properties.CreatedOn).LastOrDefault();
        if (tLevelData != null && TLevels != ProcessMode.Skip)
        {
            Console.WriteLine("Starting import of T-Level Data");

            var blobClient = containerClient.GetBlobClient(tLevelData.Name);
            Console.WriteLine("Fetching file");
            using var reader = new StreamReader(await blobClient.OpenReadAsync(cancellationToken: cancellationToken));
            Console.WriteLine("Setting up CSV reader");
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            csv.Context.RegisterClassMap<TLevelMap>();
            Console.WriteLine("Reading data...");
            var records = csv.GetRecords<TLevel>().ToList();
            var lastUpdated = records.Max(x => x.UpdatedOn);
            var lastCreated = records.Max(x => x.CreatedOn);
            var lastDeleted = records.Max(x => x.DeletedOn);

            if (
                TLevels == ProcessMode.Force
                || dbContext.FAC_TLevels.Count() != records.Count
                || dbContext.FAC_TLevels.Max(x => x.UpdatedOn) < lastUpdated
                || dbContext.FAC_TLevels.Max(x => x.CreatedOn) < lastCreated
                || dbContext.FAC_TLevels.Max(x => x.DeletedOn) < lastDeleted
            )
            {
                Console.WriteLine($"Preparing {records.Count} records to DB...");
                await dbContext.BulkSynchronizeAsync(records, options =>
                {
                    options.BatchSize = batchSize;
                    options.BatchDelayInterval = 1000;
                    options.UseTableLock = true;
                }, cancellationToken);
            }

            Console.WriteLine("Done");

        }
        
        // Get our latest T Level Definitions file
        var tLevelDefinitionData = files.Where(blob =>
                blob.Name.Contains("TLevelDefinitions_", StringComparison.InvariantCultureIgnoreCase))
            .OrderByDescending(b => b.Properties.CreatedOn).LastOrDefault();
        if (tLevelDefinitionData != null && TLevels != ProcessMode.Skip)
        {
            Console.WriteLine("Starting import of T-Level Definition Data");

            var blobClient = containerClient.GetBlobClient(tLevelDefinitionData.Name);
            Console.WriteLine("Fetching file");
            using var reader = new StreamReader(await blobClient.OpenReadAsync(cancellationToken: cancellationToken));
            Console.WriteLine("Setting up CSV reader");
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            csv.Context.RegisterClassMap<TLevelDefinitionMap>();
            Console.WriteLine("Reading data...");
            var records = csv.GetRecords<TLevelDefinition>().ToList();

            if (
                TLevels == ProcessMode.Force
                || dbContext.FAC_TLevels.Count() != records.Count
            )
            {
                Console.WriteLine($"Preparing {records.Count} records to DB...");
                await dbContext.BulkSynchronizeAsync(records, options =>
                {
                    options.BatchSize = batchSize;
                    options.BatchDelayInterval = 1000;
                    options.UseTableLock = true;
                }, cancellationToken);
            }

            Console.WriteLine("Done");

        }

        // Get our latest all courses file
        var allCoursesData = files.Where(blob =>
                blob.Name.Contains("AllCourses", StringComparison.InvariantCultureIgnoreCase))
            .OrderByDescending(b => b.Properties.CreatedOn).LastOrDefault();
        if (allCoursesData != null && AllCourses != ProcessMode.Skip)
        {
            Console.WriteLine("Starting import of All Course Report Data");

            var blobClient = containerClient.GetBlobClient(allCoursesData.Name);
            Console.WriteLine("Fetching file");
            using var reader = new StreamReader(await blobClient.OpenReadAsync(cancellationToken: cancellationToken));
            Console.WriteLine("Setting up CSV reader");
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            Console.WriteLine("Reading data...");
            csv.Context.RegisterClassMap<AllCoursesCourseMap>();
            var records = csv.GetRecords<AllCoursesCourse>().ToList();
            var lastUpdated = records.Max(x => x.CREATED_DATE);
            var lastCreated = records.Max(x => x.UPDATED_DATE);

            if (
                AllCourses == ProcessMode.Force
                || dbContext.FAC_AllCourses.Count() != records.Count
                || dbContext.FAC_AllCourses.Max(x => x.UPDATED_DATE) < lastUpdated
                || dbContext.FAC_AllCourses.Max(x => x.CREATED_DATE) < lastCreated
            )
            {
                Console.WriteLine($"Preparing {records.Count} records to DB...");
                await dbContext.BulkSynchronizeAsync(records, options =>
                {
                    options.BatchSize = batchSize;
                    options.BatchDelayInterval = 1000;
                    options.UseTableLock = true;
                }, cancellationToken);
            }

            Console.WriteLine("Done");

        }

        List<Entry> entries;

        
        
        var tlevelquery =
            from c in dbContext.FAC_AllCourses
            join t in dbContext.FAC_TLevels on
                c.COURSE_ID equals t.TLevelId
            join td in dbContext.FAC_TLevelDefinitions on
                t.TLevelDefinitionId equals td.TLevelDefinitionId
            
            select new AiSearchEntry()
            {
                Id = c.COURSE_ID.ToString(),
                InstanceId = c.COURSE_RUN_ID.ToString(),
                Title = c.COURSE_NAME,
                // TitleVector = GetVector(c.COURSE_NAME),
                LearningAimTitle = td.Name,
                // LearningAimTitleVector = GetVector(td.Name),
                Description = c.WHO_THIS_COURSE_IS_FOR,
                // DescriptionVector = GetVector(c.WHO_THIS_COURSE_IS_FOR),
                Sector = c.SECTOR,
                // SectorVector = GetVector(c.SECTOR),
                EntryType = nameof(EntryType.Course),
                QualificationLevel = MapQualificationLevel(td.QualificationLevel),
                LearningMethod = MapLearningMethod(c.DELIVER_MODE),
                CourseHours = MapCourseHours(c.STUDY_MODE),
                StudyTime = MapStudyTime(c.ATTENDANCE_PATTERN),
                Source = nameof(SourceSystem.FAC),
                Location = c.LOCATION != null ? GeographyPoint.Create( c.LOCATION.Y, c.LOCATION.X) : null
            };

        var result = tlevelquery.Take(50).ToList();

        foreach (var aiSearchEntry in result)
        {
            aiSearchEntry.TitleVector = searchIndexHandler.GetVector(aiSearchEntry.Title);
            aiSearchEntry.DescriptionVector = searchIndexHandler.GetVector(aiSearchEntry.Description);
            aiSearchEntry.LearningAimTitleVector = searchIndexHandler.GetVector(aiSearchEntry.LearningAimTitle);
            aiSearchEntry.SectorVector = searchIndexHandler.GetVector(aiSearchEntry.Sector);
        }
        
        
        Console.WriteLine(searchIndexHandler.Ingest(result));
            
        

        Console.WriteLine("FAC Ingestion Done");

        return true;
    }

    private static string MapStudyTime(AttendancePattern? attendancePattern)
    {
        return attendancePattern switch
        {
            AttendancePattern.Daytime => nameof(StudyTime.Daytime),
            AttendancePattern.Weekend => nameof(StudyTime.Weekend),
            AttendancePattern.Evening => nameof(StudyTime.Evening),
            _ => string.Empty
        };
    }

    private static string MapCourseHours(StudyMode? studyMode)
    {
        return studyMode switch
        {
            StudyMode.Flexible => nameof(CourseHours.Flexible),
            StudyMode.FullTime => nameof(CourseHours.FullTime),
            StudyMode.PartTime => nameof(CourseHours.PartTime),
            _ => string.Empty
        };
    }

    private static string MapLearningMethod(DeliveryMode? deliveryMode)
    {
        return deliveryMode switch
        {
            DeliveryMode.BlendedLearning => nameof(LearningMethod.Hybrid),
            DeliveryMode.ClassroomBased => nameof(LearningMethod.ClassroomBased),
            DeliveryMode.Online => nameof(LearningMethod.Online),
            DeliveryMode.WorkBased => nameof(LearningMethod.Workbased),
            _ => string.Empty
        };
    }

    private static string MapQualificationLevel(EducationLevel? qualificationLevel)
    {
        return qualificationLevel switch
        {
            EducationLevel.Level1 => nameof(QualificationLevel.Level1),
            EducationLevel.Level2 => nameof(QualificationLevel.Level2),
            EducationLevel.Level3 => nameof(QualificationLevel.Level3),
            EducationLevel.Level4 => nameof(QualificationLevel.Level4),
            EducationLevel.Level5 => nameof(QualificationLevel.Level5),
            EducationLevel.Level6 => nameof(QualificationLevel.Level6),
            EducationLevel.Level7 => nameof(QualificationLevel.Level7),
            EducationLevel.Level8 => nameof(QualificationLevel.Level8),
            EducationLevel.Level0 => nameof(QualificationLevel.Entry),
            _ => string.Empty
        };
    }
}