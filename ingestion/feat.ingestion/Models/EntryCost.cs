using System;
using System.ComponentModel.DataAnnotations.Schema;
namespace feat.ingestion.Models;

[Table("EntryCost")]
public class EntryCost
{
    public Guid Id { get; set; }

    public Guid EntryId { get; set; }

    public double? Value { get; set; }

    public string? Description { get; set; }

    public Entry Entry { get; set; } = null!;
}

//[ForeignKey("EntryId")]