using AnalysisService.Application.Contracts;
using MassTransit;
using AnalysisService.Application.Interfaces;
using AnalysisService.Domain.Entities;

namespace AnalysisService.Application.Consumers
{
    public class FeedbackSubmittedConsumer : IConsumer<FeedbackSubmittedEvent>
    {
        private readonly ILogger<FeedbackSubmittedConsumer> _logger;
        private readonly IAnalysisRepository _analysisRepository;

        public FeedbackSubmittedConsumer(
            ILogger<FeedbackSubmittedConsumer> logger,
            IAnalysisRepository analysisRepository)
        {
            _logger = logger;
            _analysisRepository = analysisRepository;
        }

        public async Task Consume(
            ConsumeContext<FeedbackSubmittedEvent> context)
        {
            _logger.LogInformation(
                "Feedback received: {FeedbackId}",
                context.Message.FeedbackId);

            await _analysisRepository.AddAsync(
                new FeedbackAnalysis
                {
                    FeedbackId = context.Message.FeedbackId,
                    CreatedAtUtc = DateTime.UtcNow,
                    Summary = context.Message.Comment,
                    Sentiment = "Unknown",
                    Priority = Domain.Enum.PriorityType.Normal,
                    
                },
                context.CancellationToken);
        }
    }
}
