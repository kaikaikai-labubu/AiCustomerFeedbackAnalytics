using AnalysisService.Application.Interfaces;
using AnalysisService.Domain.Entities;
using MassTransit;
using MassTransit.Transports;
using Shared.Contracts.Events;

namespace AnalysisService.Application.Consumers
{
    public class FeedbackSubmittedConsumer : IConsumer<FeedbackSubmittedEvent>
    {
        private readonly ILogger<FeedbackSubmittedConsumer> _logger;
        private readonly IAnalysisRepository _analysisRepository;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ISentimentAnalysisService _sentimentAnalysisService;

        private readonly IEmbeddingService _embeddingService;
        private readonly IFeedbackAnalysisSearchService _feedbackAnalysisSearchService;

        public FeedbackSubmittedConsumer(
            ILogger<FeedbackSubmittedConsumer> logger,
            IAnalysisRepository analysisRepository,
            IPublishEndpoint publishEndpoint,
            ISentimentAnalysisService sentimentAnalysisService,
            IEmbeddingService embeddingService,
            IFeedbackAnalysisSearchService feedbackAnalysisSearchService)
        {
            _logger = logger;
            _analysisRepository = analysisRepository;
            _publishEndpoint = publishEndpoint;
            _sentimentAnalysisService = sentimentAnalysisService;
            _embeddingService = embeddingService;
            _feedbackAnalysisSearchService = feedbackAnalysisSearchService;
        }

        public async Task Consume(
            ConsumeContext<FeedbackSubmittedEvent> context)
        {
            _logger.LogInformation(
                "Feedback received: {FeedbackId}",
                context.Message.FeedbackId);

            var sentimentResult = await _sentimentAnalysisService.AnalyzeAsync(context.Message.Comment);

            var embedding = await _embeddingService.GetEmbeddingAsync(context.Message.Comment);

            var analysis = new FeedbackAnalysis
            {
                FeedbackId = context.Message.FeedbackId,
                CreatedAtUtc = DateTime.UtcNow,
                Summary = context.Message.Comment,
                Sentiment = sentimentResult.Sentiment,
                Priority = sentimentResult.Priority,
                Confidence = sentimentResult.Confidence,
                PrimaryTopic = sentimentResult.PrimaryTopic,
                Reason = sentimentResult.Reason,
                Embedding = embedding
            };

            _logger.LogInformation(
                "Feedback analysis completed: {FeedbackId}, Sentiment: {Sentiment}, Confidence: {Confidence}, PrimaryTopic: {PrimaryTopic}, Reason: {Reason}, Priority: {Priority}, Embedding: {Embedding}",
                analysis.FeedbackId,
                analysis.Sentiment,
                analysis.Confidence,
                analysis.PrimaryTopic,
                analysis.Reason,
                analysis.Priority,
                analysis.Embedding);

            await _analysisRepository.AddAsync(
                analysis,
                context.CancellationToken);

            var similarFeedbacks = await _feedbackAnalysisSearchService.SearchSimilarAsync(
                embedding,
                context.Message.Comment,
                5,
                context.CancellationToken);

            _logger.LogInformation(
                "Top {Count} similar feedbacks found for {FeedbackId}.",
                similarFeedbacks.Count,
                analysis.FeedbackId);

            var similarFeedbackPayload = similarFeedbacks
                .Select(x => new
                {
                    x.FeedbackId,
                    Comment = x.Summary,
                    x.Score
                })
                .ToList();

            _logger.LogInformation(
                "Similar feedback matches for {FeedbackId}: {@SimilarFeedbacks}",
                analysis.FeedbackId,
                similarFeedbackPayload);

            await _feedbackAnalysisSearchService.IndexAsync(
                analysis,
                context.CancellationToken);
        }
    }
}
