namespace AnalysisService.Application.Models
{
    public sealed class FeedbackVectorSearchRequest
    {
        public string Query { get; set; } = string.Empty;

        public int Top { get; set; } = 5;
    }
}
