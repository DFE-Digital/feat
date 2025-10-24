using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Azure.Search.Documents.Models;
using feat.common.Configuration;
using feat.common.Models.AiSearch;
using Microsoft.Extensions.Options;
using OpenAI.Embeddings;

namespace feat.ingestion.Handlers;

public class SearchIndexHandler(
    IOptionsMonitor<AzureOptions> options,
    SearchIndexClient aiSearchClient)
    : ISearchIndexHandler
{
    private readonly AzureOptions _azureOptions = options.CurrentValue;

    public bool CreateIndex()
    {
        Console.WriteLine("Creating index...");
        
        var fieldBuilder = new FieldBuilder();
        string vectorSearchProfileName = "my-vector-profile";
        string vectorSearchHnswConfig = "hnsw-m10-const1000-search1000";

        var index = new SearchIndex(_azureOptions.AiSearchIndex)
        {
            Fields = fieldBuilder.Build(typeof(AiSearchEntry)),
            SemanticSearch = new SemanticSearch()
            {
                Configurations = { new SemanticConfiguration("semantic-title-description", 
                    new SemanticPrioritizedFields()
                    {
                        TitleField = new SemanticField(nameof(AiSearchEntry.Title)),
                        ContentFields =
                        {
                            new SemanticField(nameof(AiSearchEntry.Description)),
                            new SemanticField(nameof(AiSearchEntry.LearningAimTitle))
                        },
                        KeywordsFields =
                        {
                            new SemanticField(nameof(AiSearchEntry.Sector))
                        }
                    }
                    ) 
                }
            },
            VectorSearch = new VectorSearch()
            {
                Profiles = { new VectorSearchProfile(vectorSearchProfileName, vectorSearchHnswConfig) },
                Algorithms =
                {
                    new HnswAlgorithmConfiguration(vectorSearchHnswConfig)
                    {
                        Parameters = new HnswParameters()
                        {
                            EfConstruction = 1000,
                            EfSearch = 1000,
                            M = 10,
                            Metric = VectorSearchAlgorithmMetric.Cosine
                        }
                    }
                }
            }
        };
        
        var result = aiSearchClient.CreateOrUpdateIndex(index);

        Console.WriteLine("Done.");
        
        return true;
    }

    public bool Ingest(List<AiSearchEntry> entries)
    {
        var searchClient = aiSearchClient.GetSearchClient(_azureOptions.AiSearchIndex);
        var result = searchClient.MergeOrUploadDocuments(entries);
        return result.Value.Results.All(x => x.Succeeded);
    }
}