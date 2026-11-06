using FeedbackService.Application.Interfaces;
using FeedbackService.Domain.Entities;
using FeedbackService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FeedbackService.Infrastructure.Repositories;

public sealed class FeedbackRepository(FeedbackDbContext dbContext) : IFeedbackRepository
{
    public async Task<IReadOnlyList<Feedback>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Feedback
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<Feedback?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await dbContext.Feedback
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<Feedback> AddAsync(Feedback feedback, CancellationToken cancellationToken)
    {
        dbContext.Feedback.Add(feedback);
        await dbContext.SaveChangesAsync(cancellationToken);
        return feedback;
    }
}