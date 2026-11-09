using FeedbackService.Application.Interfaces;
using FeedbackService.Application.Models;
using FeedbackService.Domain.Entities;

namespace FeedbackService.Application.Services;

public sealed class FeedbackAppService(IFeedbackRepository feedbackRepository) : IFeedbackService
{
    public Task<IReadOnlyList<Feedback>> GetAllAsync(CancellationToken cancellationToken)
    {
        return feedbackRepository.GetAllAsync(cancellationToken);
    }

    public Task<Feedback?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return feedbackRepository.GetByIdAsync(id, cancellationToken);
    }

    public Task<Feedback> CreateAsync(CreateFeedbackRequest request, CancellationToken cancellationToken)
    {
        var feedback = new Feedback
        {
            Id = Guid.NewGuid(),
            CustomerId = request.CustomerId.Trim(),
            Comment = string.IsNullOrWhiteSpace(request.Comment) ? null : request.Comment.Trim(),
            CreatedAtUtc = DateTime.UtcNow
        };

        return feedbackRepository.AddAsync(feedback, cancellationToken);
    }
}