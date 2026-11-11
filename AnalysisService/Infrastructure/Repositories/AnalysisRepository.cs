using AnalysisService.Domain.Entities;
using AnalysisService.Infrastructure.Data;
using AnalysisService.Application.Interfaces;
using AnalysisService.Application.Models;
using Microsoft.EntityFrameworkCore;

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

        public async Task<IList<FeedbackAnalysis>> GetFeedbackAnalysisByFiltersAsync(GetFeedbackRequest request, CancellationToken cancellationToken)
        {
            var query = dbContext.FeedbackAnalysis.AsQueryable();

            if (request.FeedbackId.HasValue)
            {
                query = query.Where(f => f.FeedbackId == request.FeedbackId.Value);
            }

            if (request.MaxConfidence.HasValue)
            {
                query = query.Where(f => f.Confidence <= request.MaxConfidence.Value);
            }

            if (request.Priority.HasValue)
            {
                query = query.Where(f => f.Priority == request.Priority.Value);
            }

            var analysis = await query.ToListAsync(cancellationToken);

            return analysis;
        }
    }
}
