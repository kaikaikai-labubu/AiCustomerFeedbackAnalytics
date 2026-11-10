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

        public FeedbackSubmittedConsumer(
            ILogger<FeedbackSubmittedConsumer> logger,
            IAnalysisRepository analysisRepository,
            IPublishEndpoint publishEndpoint,
            ISentimentAnalysisService sentimentAnalysisService)
        {
            _logger = logger;
            _analysisRepository = analysisRepository;
            _publishEndpoint = publishEndpoint;
            _sentimentAnalysisService = sentimentAnalysisService;
        }

        public async Task Consume(
            ConsumeContext<FeedbackSubmittedEvent> context)
        {
            _logger.LogInformation(
                "Feedback received: {FeedbackId}",
                context.Message.FeedbackId);

            var sentimentResult = await _sentimentAnalysisService.AnalyzeAsync(context.Message.Comment);

            var analysis = new FeedbackAnalysis
            {
                FeedbackId = context.Message.FeedbackId,
                CreatedAtUtc = DateTime.UtcNow,
                Summary = context.Message.Comment,
                Sentiment = sentimentResult.Sentiment,
                Priority = Domain.Enum.PriorityType.Normal,

            };

            await _analysisRepository.AddAsync(
                analysis,
                context.CancellationToken);

            await _publishEndpoint.Publish(
                new FeedbackAnalysedEvent(
                    analysis.FeedbackId,
                    analysis.Sentiment,
                    analysis.Summary,
                    DateTime.UtcNow));
        }
    }
}
