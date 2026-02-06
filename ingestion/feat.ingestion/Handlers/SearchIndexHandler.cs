using System.Security.Cryptography;
using System.Text;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using feat.common.Configuration;
using feat.common.Models.AiSearch;
using Microsoft.Extensions.Options;
using OpenAI.Embeddings;
using ZiggyCreatures.Caching.Fusion;

namespace feat.ingestion.Handlers;

public class SearchIndexHandler(
    IOptionsMonitor<AzureOptions> options,
    SearchIndexClient aiSearchClient,
    EmbeddingClient embeddingClient,
    IFusionCache cache)
    : ISearchIndexHandler
{
    private readonly AzureOptions _azureOptions = options.CurrentValue;

    public bool CreateIndex()
    {
        Console.WriteLine("Creating index...");
        
        var fieldBuilder = new FieldBuilder();
        const string vectorSearchProfileName = "my-vector-profile";
        const string vectorSearchHnswConfig = "hnsw-m10-const1000-search1000";

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
        
        aiSearchClient.CreateOrUpdateIndex(index);

        Console.WriteLine("Done.");
        
        return true;
    }

    public async Task<bool> Ingest(List<AiSearchEntry> entries, CancellationToken cancellationToken)
    {
        var searchClient = aiSearchClient.GetSearchClient(_azureOptions.AiSearchIndex);
        await using SearchIndexingBufferedSender<AiSearchEntry> indexer =
            new (searchClient, new SearchIndexingBufferedSenderOptions<AiSearchEntry>()
        {
            InitialBatchActionCount = 100,
        });
        
        await indexer.MergeOrUploadDocumentsAsync(entries, cancellationToken);

        await indexer.FlushAsync(cancellationToken);
        
        return true;
    }
    
    public IReadOnlyList<float> GetVector(string? text)
    {
        if (string.IsNullOrEmpty(text) || string.IsNullOrWhiteSpace(text))
        {
            return new List<float>();
        }

        // If we have large text, let's not try and cache it
        if (text.Length > 500)
        {
            var result = embeddingClient.GenerateEmbedding(text.ToLowerInvariant());
            return result.Value.ToFloats().ToArray();
        }
        
        using var sha256Hash = SHA256.Create();
        var hash = GetHash(sha256Hash, text.ToLowerInvariant());

        var floats = cache.GetOrSet<IReadOnlyList<float>>(
            $"{hash}",
            entry =>
            {
                var result = embeddingClient.GenerateEmbedding(text.ToLowerInvariant(), cancellationToken: entry);
                return result.Value.ToFloats().ToArray();
            }
        );

        return floats;
    }

    public async Task Delete(IEnumerable<string> idsToDelete, CancellationToken cancellationToken)
    {
        var list = idsToDelete.ToList();
        if (list.Count == 0)
        {
            return;
        }

        var searchClient = aiSearchClient.GetSearchClient(_azureOptions.AiSearchIndex);
        foreach (var batch in list.Chunk(250))
        {
            await searchClient.DeleteDocumentsAsync("InstanceId", batch, cancellationToken: cancellationToken);

        }
    }

    public async Task<bool> Update(List<AiSearchEntry> entries, CancellationToken cancellationToken)
    {
        var searchClient = aiSearchClient.GetSearchClient(_azureOptions.AiSearchIndex);
        var updatedEntries = new List<AiSearchEntry>();
        
        foreach (var aiSearchEntry in entries)
        {
            var entry = await searchClient.GetDocumentAsync<AiSearchEntry>(aiSearchEntry.InstanceId, cancellationToken: cancellationToken);
            if (entry != null)
            {
                var oldEntry = entry.Value;
                if (oldEntry.Title != aiSearchEntry.Title)
                {
                    oldEntry.Title = aiSearchEntry.Title;
                    oldEntry.TitleVector = GetVector(aiSearchEntry.Title);
                }
                if (oldEntry.Description != aiSearchEntry.Description)
                {
                    oldEntry.Description = aiSearchEntry.Description;
                    oldEntry.DescriptionVector = GetVector(aiSearchEntry.Description);
                }
                if (oldEntry.LearningAimTitle != aiSearchEntry.LearningAimTitle)
                {
                    oldEntry.LearningAimTitle = aiSearchEntry.LearningAimTitle;
                    oldEntry.LearningAimTitleVector = GetVector(aiSearchEntry.LearningAimTitle);
                }
                if (oldEntry.Sector != aiSearchEntry.Sector)
                {
                    oldEntry.Sector = aiSearchEntry.Sector;
                    oldEntry.SectorVector = GetVector(aiSearchEntry.Sector);
                }
                
                oldEntry.Location = aiSearchEntry.Location;
                oldEntry.CourseHours = aiSearchEntry.CourseHours;
                oldEntry.CourseType  = aiSearchEntry.CourseType;
                oldEntry.EntryType = aiSearchEntry.EntryType;
                oldEntry.LearningMethod =  aiSearchEntry.LearningMethod;
                oldEntry.QualificationLevel =  aiSearchEntry.QualificationLevel;
                oldEntry.Source =  aiSearchEntry.Source;
                oldEntry.StudyTime = aiSearchEntry.StudyTime;
                oldEntry.IsNational = aiSearchEntry.IsNational;
                
                updatedEntries.Add(oldEntry);
            }
        }

        return await Ingest(updatedEntries, cancellationToken);
    }

    private static string GetHash(HashAlgorithm hashAlgorithm, string input)
    {

        // Convert the input string to a byte array and compute the hash.
        var data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));

        // Create a new Stringbuilder to collect the bytes
        // and create a string.
        var sBuilder = new StringBuilder();

        // Loop through each byte of the hashed data
        // and format each one as a hexadecimal string.
        foreach (var t in data)
        {
            sBuilder.Append(t.ToString("x2"));
        }

        // Return the hexadecimal string.
        return sBuilder.ToString();
    }

}