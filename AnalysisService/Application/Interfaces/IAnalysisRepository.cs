using AnalysisService.Domain.Entities;

namespace AnalysisService.Application.Interfaces
{
    public interface IAnalysisRepository
    {
        Task<FeedbackAnalysis> AddAsync(FeedbackAnalysis analysis, CancellationToken cancellationToken);
    }
}
