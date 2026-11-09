namespace FeedbackService.Application.Contracts
{
    public record FeedbackSubmittedEvent(
        Guid FeedbackId,
        string CustomerId,
        string Comment
    );
}
