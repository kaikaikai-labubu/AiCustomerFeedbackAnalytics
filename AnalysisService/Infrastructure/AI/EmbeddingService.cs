using AnalysisService.Application.Interfaces;
using Microsoft.Extensions.Options;
using OpenAI;
using OpenAI.Embeddings;
using System.ClientModel;

namespace AnalysisService.Infrastructure.AI
{
    public sealed class EmbeddingService : IEmbeddingService
    {
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

            var embeddingDeploymentName = string.IsNullOrWhiteSpace(_options.EmbeddingDeploymentName)
                ? _options.DeploymentName
                : _options.EmbeddingDeploymentName;

            if (string.IsNullOrWhiteSpace(embeddingDeploymentName))
            {
                throw new InvalidOperationException("AzureOpenAI:EmbeddingDeploymentName is not configured.");
            }

            EmbeddingClient embeddingClient = client.GetEmbeddingClient(embeddingDeploymentName);
            OpenAIEmbedding embedding = await embeddingClient.GenerateEmbeddingAsync(feedback);

            return embedding.ToFloats();
        }
    }
}
