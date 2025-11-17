using System.Security.Cryptography;
using System.Text;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Azure.Search.Documents.Models;
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

    public async Task<bool> Ingest(List<AiSearchEntry> entries)
    {
        var searchClient = aiSearchClient.GetSearchClient(_azureOptions.AiSearchIndex);
        await using SearchIndexingBufferedSender<AiSearchEntry> indexer =
            new (searchClient, new SearchIndexingBufferedSenderOptions<AiSearchEntry>()
        {
            InitialBatchActionCount = 100,
        });
        
        await indexer.MergeOrUploadDocumentsAsync(entries);

        await indexer.FlushAsync();
        
        return true;
    }
    
    public IReadOnlyList<float> GetVector(string? text)
    {
        if (string.IsNullOrEmpty(text) || string.IsNullOrWhiteSpace(text))
        {
            return new List<float>();
        }

        // If we have large text, let's not try and cache it
        if (text?.Length > 200)
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
    
    private static string GetHash(HashAlgorithm hashAlgorithm, string input)
    {

        // Convert the input string to a byte array and compute the hash.
        byte[] data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));

        // Create a new Stringbuilder to collect the bytes
        // and create a string.
        var sBuilder = new StringBuilder();

        // Loop through each byte of the hashed data
        // and format each one as a hexadecimal string.
        for (int i = 0; i < data.Length; i++)
        {
            sBuilder.Append(data[i].ToString("x2"));
        }

        // Return the hexadecimal string.
        return sBuilder.ToString();
    }

    // Verify a hash against a string.
    private static bool VerifyHash(HashAlgorithm hashAlgorithm, string input, string hash)
    {
        // Hash the input.
        var hashOfInput = GetHash(hashAlgorithm, input);

        // Create a StringComparer an compare the hashes.
        StringComparer comparer = StringComparer.OrdinalIgnoreCase;

        return comparer.Compare(hashOfInput, hash) == 0;
    }
}