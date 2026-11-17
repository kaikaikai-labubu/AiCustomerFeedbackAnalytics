using AnalysisService.Application.Interfaces;
using Microsoft.Extensions.Options;
using OpenAI;
using OpenAI.Embeddings;
using System.ClientModel;

namespace AnalysisService.Infrastructure.AI
{
    public sealed class EmbeddingService : IEmbeddingService
    {
        private const string EmbeddingModel = "text-embedding-3-small";
        private readonly AzureOpenAiOptions _options;

        public EmbeddingService(IOptions<AzureOpenAiOptions> options)
        {
            _options = options.Value;
        }

        public async Task<ReadOnlyMemory<float>> GetEmbeddingAsync(string feedback)
        {
            if (string.IsNullOrWhiteSpace(feedback))
            {
                throw new ArgumentException("Feedback cannot be empty.", nameof(feedback));
            }

            var client = new OpenAIClient(
                new ApiKeyCredential(_options.ApiKey),
                new OpenAIClientOptions
                {
                    Endpoint = new Uri(_options.Endpoint)
                });

            EmbeddingClient embeddingClient = client.GetEmbeddingClient(EmbeddingModel);
            OpenAIEmbedding embedding = await embeddingClient.GenerateEmbeddingAsync(feedback);

            return embedding.ToFloats();
        }
    }
}
