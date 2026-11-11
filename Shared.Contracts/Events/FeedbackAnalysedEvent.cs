namespace Shared.Contracts.Events
{
    public record FeedbackAnalysedEvent(
    Guid FeedbackId,
    string Sentiment,
    string Summary,
    DateTime AnalysedAtUtc,
    string Reason,
    string PrimaryTopic,
    double Confidence,
    string Priority);

}
