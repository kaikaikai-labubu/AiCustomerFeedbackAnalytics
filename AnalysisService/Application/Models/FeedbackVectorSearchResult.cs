namespace AnalysisService.Application.Models
{
    public sealed class FeedbackVectorSearchResult
    {
        public Guid Id { get; set; }

        public Guid FeedbackId { get; set; }

        public string Sentiment { get; set; } = string.Empty;

        public string Summary { get; set; } = string.Empty;

        public double Confidence { get; set; }

        public string PrimaryTopic { get; set; } = string.Empty;

        public string Reason { get; set; } = string.Empty;

        public DateTimeOffset CreatedAtUtc { get; set; }

        public string Priority { get; set; } = string.Empty;

        public double? Score { get; set; }
    }
}
