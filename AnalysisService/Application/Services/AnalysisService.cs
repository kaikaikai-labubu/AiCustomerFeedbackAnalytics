using AnalysisService.Application.Interfaces;
using AnalysisService.Application.Models;
using AnalysisService.Domain.Entities;

namespace AnalysisService.Application.Services
{
    public sealed class AnalysisService(IAnalysisRepository analysisRepository)
    {
        public async Task<IList<FeedbackAnalysis>> GetAnalysisBySentimentScoreAsync(GetFeedbackRequest request, CancellationToken cancellationToken)
        {            
            return await analysisRepository.GetFeedbackAnalysisByFiltersAsync(request, cancellationToken);
        }
    }
}
