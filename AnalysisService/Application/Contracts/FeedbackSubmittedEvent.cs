namespace AnalysisService.Application.Contracts
{
    public record FeedbackSubmittedEvent(
        Guid FeedbackId,
        string CustomerId,
        string Comment
    );
}
