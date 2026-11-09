using AnalysisService.Domain.Entities;
using AnalysisService.Infrastructure.Data;
using AnalysisService.Application.Interfaces;

namespace AnalysisService.Infrastructure.Repositories
{
    public sealed class AnalysisRepository(AnalysisDbContext dbContext) : IAnalysisRepository
    {
        public async Task<FeedbackAnalysis> AddAsync(FeedbackAnalysis analysis, CancellationToken cancellationToken)
        {
            await dbContext.FeedbackAnalysis.AddAsync(analysis, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            return analysis;
        }
    }
}
