using System.Text.Json.Serialization;
using Azure.Core.Serialization;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Microsoft.Spatial;

namespace feat.common.Models.AiSearch;

public class AiSearchEntry
{
    [SimpleField]
    public required string Id { get; set; }
    
    [SimpleField(IsKey = true)]
    public required string InstanceId { get; set; }
    
    [SearchableField(AnalyzerName = LexicalAnalyzerName.Values.EnMicrosoft)]
    public required string Title { get; set; }
    
    [VectorSearchField(IsHidden = true, VectorSearchDimensions = 3072, VectorSearchProfileName = "my-vector-profile")]
    public IReadOnlyList<float> TitleVector { get; set; }
    
    [SearchableField(AnalyzerName = LexicalAnalyzerName.Values.EnMicrosoft)]
    public string? LearningAimTitle { get; set; }
    
    [VectorSearchField(IsHidden = true, VectorSearchDimensions = 3072, VectorSearchProfileName = "my-vector-profile")]
    public IReadOnlyList<float> LearningAimTitleVector { get; set; }
    
    [SearchableField(AnalyzerName = LexicalAnalyzerName.Values.EnMicrosoft)]
    public string? Description { get; set; }
    
    [VectorSearchField(IsHidden = true, VectorSearchDimensions = 3072, VectorSearchProfileName = "my-vector-profile")]
    public IReadOnlyList<float> DescriptionVector { get; set; }
    
    [SearchableField(AnalyzerName = LexicalAnalyzerName.Values.EnMicrosoft)]
    public string? Sector { get; set; }
    
    [VectorSearchField(IsHidden = true, VectorSearchDimensions = 3072, VectorSearchProfileName = "my-vector-profile")]
    public IReadOnlyList<float> SectorVector { get; set; }
    
    [SimpleField(IsFacetable = true)]
    public required string EntryType { get; set; }
    
    [SimpleField(IsFacetable = true)]
    public required string QualificationLevel { get; set; }
    
    [SimpleField(IsFacetable = true)]
    public required string LearningMethod { get; set; }
    
    [SimpleField(IsFacetable = true)]
    public required string CourseHours { get; set; }
    
    [SimpleField(IsFacetable = true)]
    public required string StudyTime { get; set; }
    
    [SimpleField(IsFilterable = true)]
    public required string Source { get; set; }
    
    [JsonConverter(typeof(MicrosoftSpatialGeoJsonConverter))]
    [SimpleField(IsFilterable = true, IsSortable = true)]
    public required GeographyPoint? Location { get; set; }
}