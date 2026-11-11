using AnalysisService.Application.Models;
using AnalysisService.Domain.Entities;

namespace AnalysisService.Application.Interfaces
{
    public interface IAnalysisRepository
    {
        Task<FeedbackAnalysis> AddAsync(FeedbackAnalysis analysis, CancellationToken cancellationToken);

        Task<IList<FeedbackAnalysis>> GetFeedbackAnalysisByFiltersAsync(GetFeedbackRequest request, CancellationToken cancellationToken);
    }
}
