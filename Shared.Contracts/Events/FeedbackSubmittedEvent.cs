namespace Shared.Contracts.Events
{
    public record FeedbackSubmittedEvent(
        Guid FeedbackId,
        string CustomerId,
        string Comment
    );
}
