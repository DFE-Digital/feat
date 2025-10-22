using System.Text.Json.Serialization;
using Azure.Core.Serialization;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Microsoft.Spatial;

namespace feat.common.Models.AiSearch;

public class AiSearchEntry
{
    [SimpleField(IsKey = true)]
    public required string Id { get; set; }
    
    [SearchableField(AnalyzerName = LexicalAnalyzerName.Values.EnMicrosoft)]
    public required string Title { get; set; }
    
    [VectorSearchField(IsHidden = true, VectorSearchDimensions = 256, VectorSearchProfileName = "compressed")]
    public ReadOnlyMemory<float> TitleVector { get; set; }
    
    [SearchableField(AnalyzerName = LexicalAnalyzerName.Values.EnMicrosoft)]
    public string? LearningAimTitle { get; set; }
    
    [VectorSearchField(IsHidden = true, VectorSearchDimensions = 256, VectorSearchProfileName = "compressed")]
    public ReadOnlyMemory<float> LearningAimTitleVector { get; set; }
    
    [SearchableField(AnalyzerName = LexicalAnalyzerName.Values.EnMicrosoft)]
    public string? Description { get; set; }
    
    [VectorSearchField(IsHidden = true, VectorSearchDimensions = 256, VectorSearchProfileName = "compressed")]
    public ReadOnlyMemory<float> DescriptionVector { get; set; }
    
    [SearchableField(IsFacetable = true, AnalyzerName = LexicalAnalyzerName.Values.EnMicrosoft)]
    public required string[] Sectors { get; set; }
    
    [VectorSearchField(IsHidden = true, VectorSearchDimensions = 256, VectorSearchProfileName = "compressed")]
    public ReadOnlyMemory<float> SectorsVector { get; set; }
    
    [SimpleField(IsFacetable = true)]
    public required string CourseType { get; set; }
    
    [SimpleField(IsFacetable = true)]
    public required string QualificationLevel { get; set; }
    
    [SimpleField(IsFacetable = true)]
    public required string[] LearningMethod { get; set; }
    
    [SimpleField(IsFacetable = true)]
    public required string[] CourseHours { get; set; }
    
    [SimpleField(IsFacetable = true)]
    public required string[] StudyTime { get; set; }
    
    [SimpleField(IsFilterable = true)]
    public required string Source { get; set; }
    
    [JsonConverter(typeof(MicrosoftSpatialGeoJsonConverter))]
    [SimpleField(IsFilterable = true, IsSortable = true)]
    public required GeographyPoint[] Locations { get; set; }
}