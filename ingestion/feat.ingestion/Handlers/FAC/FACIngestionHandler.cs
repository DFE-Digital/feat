using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using feat.ingestion.Configuration;
using feat.ingestion.Enums;
using Microsoft.Extensions.Options;

namespace feat.ingestion.Handlers.FAC;

public class FACIngestionHandler(IngestionOptions options) : IngestionHandler(options)
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

        
        
        return true;
    }
}