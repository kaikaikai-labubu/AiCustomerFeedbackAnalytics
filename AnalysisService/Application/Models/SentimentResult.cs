using AnalysisService.Domain.Enum;

namespace AnalysisService.Application.Models
{
    public record SentimentResult(
        string Sentiment,
        double Confidence,
        string PrimaryTopic,
        string Reason,
        PriorityType Priority
    );
}
