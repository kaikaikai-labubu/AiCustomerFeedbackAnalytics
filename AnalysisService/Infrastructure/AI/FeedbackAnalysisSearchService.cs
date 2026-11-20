using AnalysisService.Application.Interfaces;
using AnalysisService.Application.Models;
using AnalysisService.Domain.Entities;
using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using Microsoft.Extensions.Options;

namespace AnalysisService.Infrastructure.AI
{
    public sealed class FeedbackAnalysisSearchService : IFeedbackAnalysisSearchService
    {
        private const string IndexName = "feedback-analysis-index";
        private const string VectorFieldName = "embedding";

        private readonly AzureOpenAiOptions _options;

        public FeedbackAnalysisSearchService(IOptions<AzureOpenAiOptions> options)
        {
            _options = options.Value;
        }

        public async Task IndexAsync(FeedbackAnalysis analysis, CancellationToken cancellationToken)
        {
            var client = CreateSearchClient();

            var document = new SearchDocument
            {
                ["id"] = analysis.Id.ToString(),
                ["feedbackId"] = analysis.FeedbackId.ToString(),
                ["sentiment"] = analysis.Sentiment,
                ["summary"] = analysis.Summary,
                ["confidence"] = analysis.Confidence,
                ["primaryTopic"] = analysis.PrimaryTopic,
                ["reason"] = analysis.Reason,
                ["createdAtUtc"] = analysis.CreatedAtUtc,
                ["priority"] = analysis.Priority.ToString(),
                [VectorFieldName] = analysis.Embedding.ToArray()
            };

            await client.MergeOrUploadDocumentsAsync([document], cancellationToken: cancellationToken);
        }

        public async Task<IReadOnlyList<FeedbackVectorSearchResult>> SearchSimilarAsync(ReadOnlyMemory<float> embedding, string? query, int top, CancellationToken cancellationToken)
        {
            if (embedding.IsEmpty)
            {
                throw new ArgumentException("Embedding cannot be empty.", nameof(embedding));
            }

            return await SearchAsync(query, embedding, top, cancellationToken);
        }

        private async Task<IReadOnlyList<FeedbackVectorSearchResult>> SearchAsync(string? query, ReadOnlyMemory<float> embedding, int top, CancellationToken cancellationToken)
        {
            if (top <= 0)
            {
                top = 5;
            }

            var client = CreateSearchClient();

            var searchOptions = new SearchOptions
            {
                Size = top
            };

            if (!string.IsNullOrWhiteSpace(query))
            {
                searchOptions.SearchFields.Add("summary");
                searchOptions.SearchFields.Add("reason");
                searchOptions.SearchFields.Add("primaryTopic");
                searchOptions.SearchFields.Add("sentiment");
            }

            searchOptions.Select.Add("id");
            searchOptions.Select.Add("feedbackId");
            searchOptions.Select.Add("sentiment");
            searchOptions.Select.Add("summary");
            searchOptions.Select.Add("confidence");
            searchOptions.Select.Add("primaryTopic");
            searchOptions.Select.Add("reason");
            searchOptions.Select.Add("createdAtUtc");
            searchOptions.Select.Add("priority");

            searchOptions.VectorSearch = new VectorSearchOptions();
            searchOptions.VectorSearch.Queries.Add(new VectorizedQuery(embedding.ToArray())
            {
                KNearestNeighborsCount = top,
                Fields = { VectorFieldName }
            });

            var response = await client.SearchAsync<SearchDocument>(query, searchOptions, cancellationToken);

            var results = new List<FeedbackVectorSearchResult>();
            await foreach (var item in response.Value.GetResultsAsync())
            {
                var document = item.Document;
                results.Add(new FeedbackVectorSearchResult
                {
                    Id = ParseGuid(document, "id"),
                    FeedbackId = ParseGuid(document, "feedbackId"),
                    Sentiment = ParseString(document, "sentiment"),
                    Summary = ParseString(document, "summary"),
                    Confidence = ParseDouble(document, "confidence"),
                    PrimaryTopic = ParseString(document, "primaryTopic"),
                    Reason = ParseString(document, "reason"),
                    CreatedAtUtc = ParseDateTimeOffset(document, "createdAtUtc"),
                    Priority = ParseString(document, "priority"),
                    Score = item.Score
                });
            }

            return results;
        }

        private SearchClient CreateSearchClient()
        {
            if (string.IsNullOrWhiteSpace(_options.AiSearchEndpoint) || string.IsNullOrWhiteSpace(_options.AiSearchApiKey))
            {
                throw new InvalidOperationException("AI Search settings are missing. Configure AzureOpenAI:AiSearchEndpoint and AzureOpenAI:AiSearchApiKey.");
            }

            return new SearchClient(
                new Uri(_options.AiSearchEndpoint),
                IndexName,
                new AzureKeyCredential(_options.AiSearchApiKey));
        }

        private static string ParseString(SearchDocument document, string key)
        {
            if (!document.TryGetValue(key, out var value) || value is null)
            {
                return string.Empty;
            }

            return value.ToString() ?? string.Empty;
        }

        private static Guid ParseGuid(SearchDocument document, string key)
        {
            var value = ParseString(document, key);
            return Guid.TryParse(value, out var result) ? result : Guid.Empty;
        }

        private static double ParseDouble(SearchDocument document, string key)
        {
            if (!document.TryGetValue(key, out var value) || value is null)
            {
                return 0;
            }

            return Convert.ToDouble(value);
        }

        private static DateTimeOffset ParseDateTimeOffset(SearchDocument document, string key)
        {
            if (!document.TryGetValue(key, out var value) || value is null)
            {
                return DateTimeOffset.MinValue;
            }

            if (value is DateTimeOffset dto)
            {
                return dto;
            }

            if (value is DateTime dt)
            {
                return new DateTimeOffset(dt);
            }

            return DateTimeOffset.TryParse(value.ToString(), out var parsed) ? parsed : DateTimeOffset.MinValue;
        }
    }
}
