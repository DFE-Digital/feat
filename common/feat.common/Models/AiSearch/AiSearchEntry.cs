using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Azure.Core.Serialization;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using feat.common.Extensions;
using Microsoft.Spatial;
using NetTopologySuite.Geometries;

namespace feat.common.Models.AiSearch;

[Table("AiSearchEntries")]
public class AiSearchEntry
{
    [SimpleField]
    [StringLength(200)]
    public required string Id { get; set; }
    
    [SimpleField(IsKey = true)]
    [StringLength(400)]
    public required string InstanceId { get; set; }
    
    [SearchableField(AnalyzerName = LexicalAnalyzerName.Values.EnMicrosoft)]
    [StringLength(255)]
    public required string Title { get; set; }
    
    [VectorSearchField(IsHidden = false, VectorSearchDimensions = 3072, VectorSearchProfileName = "my-vector-profile")]
    [NotMapped]
    public IReadOnlyList<float> TitleVector { get; set; } = new List<float>();
    
    [SearchableField(AnalyzerName = LexicalAnalyzerName.Values.EnMicrosoft)]
    [StringLength(255)]
    public string? LearningAimTitle { get; set; }

    [VectorSearchField(IsHidden = false, VectorSearchDimensions = 3072, VectorSearchProfileName = "my-vector-profile")]
    [NotMapped]
    public IReadOnlyList<float> LearningAimTitleVector { get; set; } = new List<float>();
    
    [SearchableField(AnalyzerName = LexicalAnalyzerName.Values.EnMicrosoft)]
    public string? Description { get; set; }
    
    [VectorSearchField(IsHidden = false, VectorSearchDimensions = 3072, VectorSearchProfileName = "my-vector-profile")]
    [NotMapped]
    public IReadOnlyList<float> DescriptionVector { get; set; } = new List<float>();
    
    [SearchableField(AnalyzerName = LexicalAnalyzerName.Values.EnMicrosoft)]
    [StringLength(800)]
    public string? Sector { get; set; }
    
    [VectorSearchField(IsHidden = false, VectorSearchDimensions = 3072, VectorSearchProfileName = "my-vector-profile")]
    [NotMapped]
    public IReadOnlyList<float> SectorVector { get; set; } = new List<float>();
    
    [SimpleField(IsFacetable = true, IsFilterable = true)]
    [StringLength(50)]
    public required string EntryType { get; set; }
    
    [SimpleField(IsFacetable = true, IsFilterable = true)]
    [StringLength(50)]
    public required string CourseType { get; set; }
    
    [SimpleField(IsFacetable = true, IsFilterable = true)]
    [StringLength(50)]
    public required string QualificationLevel { get; set; }
    
    [SimpleField(IsFacetable = true, IsFilterable = true)]
    [StringLength(50)]
    public required string LearningMethod { get; set; }
    
    [SimpleField(IsFacetable = true, IsFilterable = true)]
    [StringLength(50)]
    public required string CourseHours { get; set; }
    
    [SimpleField(IsFacetable = true, IsFilterable = true)]
    [StringLength(50)]
    public required string StudyTime { get; set; }
    
    [SimpleField(IsFilterable = true)]
    [StringLength(50)]
    public required string Source { get; set; }
    
    [JsonConverter(typeof(MicrosoftSpatialGeoJsonConverter))]
    [SimpleField(IsFilterable = true, IsSortable = true)]
    [NotMapped]
    public required GeographyPoint? Location { get; set; }

    [JsonIgnore]
    [Column("Location")]
    public Point? LocationPoint
    {
        get => Location.ToPoint();
        set => Location = value.ToGeographyPoint();
    }
}