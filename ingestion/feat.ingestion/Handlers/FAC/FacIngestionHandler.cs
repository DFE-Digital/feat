using System.Globalization;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using CsvHelper;
using feat.common.Models.Staging.FAC;
using feat.ingestion.Configuration;
using feat.ingestion.Data;
using feat.ingestion.Enums;
using Microsoft.Extensions.Options;

namespace feat.ingestion.Handlers.FAC;

public class FacIngestionHandler(
    IngestionOptions options, 
    IngestionDbContext dbContext) 
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
        ProcessMode Aim = ProcessMode.Process;
        ProcessMode Courses = ProcessMode.Process;
        ProcessMode TLevels = ProcessMode.Process;
        ProcessMode AllCourses = ProcessMode.Process;
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
                || dbContext.FAC_TLevels.Count() != records.Count
                || dbContext.FAC_TLevels.Max(x => x.UpdatedOn) < lastUpdated
                || dbContext.FAC_TLevels.Max(x => x.CreatedOn) < lastCreated
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

        Console.WriteLine("FAC Ingestion Done");

        return true;
    }
}