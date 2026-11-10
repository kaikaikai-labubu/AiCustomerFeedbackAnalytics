using AnalysisService.Application.Interfaces;
using AnalysisService.Application.Models;
using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Options;
using OpenAI.Chat;
using System.Text.Json;

namespace AnalysisService.Infrastructure.AI
{
    public class SentimentAnalysisService : ISentimentAnalysisService
    {
        private readonly AzureOpenAiOptions _options;
        private readonly Logger<SentimentAnalysisService> _logger;

        public SentimentAnalysisService(
            IOptions<AzureOpenAiOptions> options, ILogger<SentimentAnalysisService> logger)
        {
            _options = options.Value;
            _logger = logger;
        }
        public async Task<SentimentResult> AnalyzeAsync(string feedback)
        {
            if (string.IsNullOrWhiteSpace(feedback))
            {
                throw new ArgumentException("Feedback cannot be empty.", nameof(feedback));
            }

            var client = new AzureOpenAIClient(
            new Uri(_options.Endpoint),
            new AzureKeyCredential(_options.ApiKey));

            var chatClient =
                client.GetChatClient(_options.DeploymentName);

            var result = await chatClient.CompleteChatAsync(
            [
                new SystemChatMessage("""
                    You are a customer feedback sentiment classifier.

                    Analyze customer feedback and return JSON only.

                    Rules:
                    - sentiment must be: positive, neutral, negative, or mixed
                    - confidence must be between 0 and 1
                    - primaryTopic should be a short topic
                    - reason should be under 30 words

                    Response format:

                    {
                      "sentiment": "positive",
                      "confidence": 0.95,
                      "primaryTopic": "Application Performance",
                      "reason": "Customer is satisfied with speed and usability."
                    }
                    """),

            new UserChatMessage(feedback)
            ]);

            var response =
                result.Value.Content[0].Text;

            _logger.LogInformation(
                "Sentiment analysis result: {Response}",
                response);

            return JsonSerializer.Deserialize<SentimentResult>(
                response,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
        }
    }
}
