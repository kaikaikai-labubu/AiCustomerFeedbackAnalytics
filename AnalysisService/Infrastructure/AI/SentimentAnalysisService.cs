using AnalysisService.Application.Interfaces;
using AnalysisService.Application.Models;
using Microsoft.Extensions.Options;
using OpenAI;
using OpenAI.Chat;
using System.ClientModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AnalysisService.Infrastructure.AI
{
    public class SentimentAnalysisService : ISentimentAnalysisService
    {
        private readonly AzureOpenAiOptions _options;
        private readonly ILogger<SentimentAnalysisService> _logger;

        public SentimentAnalysisService(
            IOptions<AzureOpenAiOptions> options, ILogger<SentimentAnalysisService> logger)
        {
            _options = options.Value;
            _logger = logger;
        }
        public async Task<SentimentResult> AnalyzeAsync(string feedback)
        {
            _logger.LogInformation(
                "Analyzing feedback: {Feedback}",
                feedback);

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

            var chatClient = client.GetChatClient(_options.DeploymentName);

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
                    - priority must be: Low, Normal, or High

                    Response format:

                    {
                      "sentiment": "positive",
                      "confidence": 0.95,
                      "primaryTopic": "Application Performance",
                      "reason": "Customer is satisfied with speed and usability.",
                      "priority": "Low"
                    }
                    """),
                new UserChatMessage(feedback)
            ]);

            var response = result.Value.Content[0].Text;

            _logger.LogInformation(
                "Sentiment analysis result: {Response}",
                response);

            var sentimentResult = JsonSerializer.Deserialize<SentimentResult>(
                response,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters = { new JsonStringEnumConverter() }
                });

            if (sentimentResult is null)
            {
                throw new InvalidOperationException("OpenAI response could not be parsed into SentimentResult.");
            }

            return sentimentResult;
        }
    }
}
