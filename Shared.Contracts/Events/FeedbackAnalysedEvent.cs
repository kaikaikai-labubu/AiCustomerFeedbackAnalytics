namespace Shared.Contracts.Events
{
    public record FeedbackAnalysedEvent(
    Guid FeedbackId,
    string Sentiment,
    string Summary,
    DateTime AnalysedAtUtc);

}
