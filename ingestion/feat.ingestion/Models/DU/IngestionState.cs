using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Azure;

namespace feat.ingestion.Models.DU;

[Table("DU_IngestionState")]
public class IngestionState
{
    [Key] public required Guid Id { get; set; } = Guid.NewGuid();
    
    public string? ETag { get; set; }

    public required bool DownloadComplete { get; set; } = false;

    public required bool Extracted { get; set; } = false;
    
}