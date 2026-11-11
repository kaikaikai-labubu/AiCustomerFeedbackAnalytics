using AnalysisService.Domain.Enum;

namespace AnalysisService.Application.Models
{
    public sealed class GetFeedbackRequest
    {
        public Guid? FeedbackId { get; set; }

        public double? MaxConfidence { get; set; } = 1.0;

        public PriorityType? Priority { get; set; }
    }
}
