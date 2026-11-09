using AnalysisService.Domain.Enum;

namespace AnalysisService.Domain.Entities
{
    public class FeedbackAnalysis
    {
        public Guid Id { get; set; }

        public Guid FeedbackId { get; set; }

        public string Sentiment { get; set; } = "Unknown";

        public string Summary { get; set; } = string.Empty;

        public DateTime CreatedAtUtc { get; set; }

        public PriorityType Priority { get; set; }
    }
}
