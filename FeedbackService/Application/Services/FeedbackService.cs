using FeedbackService.Application.Interfaces;
using FeedbackService.Application.Models;
using FeedbackService.Domain.Entities;
using MassTransit;
using FeedbackService.Application.Contracts;

namespace FeedbackService.Application.Services;

public sealed class FeedbackAppService(IFeedbackRepository feedbackRepository, IPublishEndpoint publisher) : IFeedbackService
{
    public Task<IReadOnlyList<Feedback>> GetAllAsync(CancellationToken cancellationToken)
    {
        return feedbackRepository.GetAllAsync(cancellationToken);
    }

    public Task<Feedback?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return feedbackRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<Feedback> CreateAsync(CreateFeedbackRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        cancellationToken.ThrowIfCancellationRequested();

        var customerId = request.CustomerId?.Trim();
        if (string.IsNullOrWhiteSpace(customerId))
        {
            throw new ArgumentException("CustomerId is required.", nameof(request));
        }

        var feedback = new Feedback
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            Comment = string.IsNullOrWhiteSpace(request.Comment) ? null : request.Comment.Trim(),
            CreatedAtUtc = DateTime.UtcNow
        };

        var created = await feedbackRepository.AddAsync(feedback, cancellationToken);

        await publisher.Publish(new FeedbackSubmittedEvent(
            created.Id,
            created.CustomerId,
            created.Comment ?? string.Empty
        ), cancellationToken);

        return created;
    }
}