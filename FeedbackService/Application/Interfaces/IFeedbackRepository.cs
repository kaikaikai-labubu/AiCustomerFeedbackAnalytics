using FeedbackService.Domain.Entities;

namespace FeedbackService.Application.Interfaces;

public interface IFeedbackRepository
{
    Task<IReadOnlyList<Feedback>> GetAllAsync(CancellationToken cancellationToken);
    Task<Feedback?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Feedback> AddAsync(Feedback feedback, CancellationToken cancellationToken);
}