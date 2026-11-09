using AnalysisService.Application.Contracts;
using MassTransit;

namespace AnalysisService.Application.Consumers
{
    public class FeedbackSubmittedConsumer
    : IConsumer<FeedbackSubmittedEvent>
    {
        private readonly ILogger<FeedbackSubmittedConsumer> _logger;

        public FeedbackSubmittedConsumer(
            ILogger<FeedbackSubmittedConsumer> logger)
        {
            _logger = logger;
        }

        public Task Consume(
            ConsumeContext<FeedbackSubmittedEvent> context)
        {
            _logger.LogInformation(
                "Feedback received: {FeedbackId}",
                context.Message.FeedbackId);

            return Task.CompletedTask;
        }
    }
}
