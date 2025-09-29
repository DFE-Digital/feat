﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace feat.ingestion.Models;

[Table("EntryCost")]
public class EntryCost
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid EntryId { get; set; }

    public double? Value { get; set; }

    public string? Description { get; set; }

    [ForeignKey("EntryId")]
    [InverseProperty("EntryCosts")]
    public Entry Entry { get; set; } = null!;
}