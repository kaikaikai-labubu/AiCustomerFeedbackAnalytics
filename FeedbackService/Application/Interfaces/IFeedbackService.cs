using FeedbackService.Application.Models;
using FeedbackService.Domain.Entities;

namespace FeedbackService.Application.Interfaces;

public interface IFeedbackService
{
    Task<IReadOnlyList<Feedback>> GetAllAsync(CancellationToken cancellationToken);
    Task<Feedback?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Feedback> CreateAsync(CreateFeedbackRequest request, CancellationToken cancellationToken);
}