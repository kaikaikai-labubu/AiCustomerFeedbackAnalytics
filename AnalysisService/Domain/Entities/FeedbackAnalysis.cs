using AnalysisService.Domain.Enum;

namespace AnalysisService.Domain.Entities
{
    public class FeedbackAnalysis
    {
        public Guid Id { get; set; }

        public Guid FeedbackId { get; set; }

        public string Sentiment { get; set; } = "Unknown";

        public string Summary { get; set; } = string.Empty;

        public double Confidence { get; set; }

        public string PrimaryTopic { get; set; } = string.Empty;

        public string Reason { get; set; } = string.Empty;

        public DateTime CreatedAtUtc { get; set; }

        public PriorityType Priority { get; set; }
    }
}
