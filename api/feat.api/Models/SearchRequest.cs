using System.ComponentModel.DataAnnotations;

namespace feat.api.Models;

public class SearchRequest
{
    [Required]
    public required string Query { get; set; }
    
    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 10;
}