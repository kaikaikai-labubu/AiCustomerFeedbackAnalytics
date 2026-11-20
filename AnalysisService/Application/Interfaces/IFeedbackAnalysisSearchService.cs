using AnalysisService.Application.Models;
using AnalysisService.Domain.Entities;

namespace AnalysisService.Application.Interfaces
{
    public interface IFeedbackAnalysisSearchService
    {
        Task IndexAsync(FeedbackAnalysis analysis, CancellationToken cancellationToken);

        Task<IReadOnlyList<FeedbackVectorSearchResult>> SearchSimilarAsync(ReadOnlyMemory<float> embedding, string? query, int top, CancellationToken cancellationToken);
    }
}
