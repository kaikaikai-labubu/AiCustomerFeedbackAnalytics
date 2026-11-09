namespace FeedbackService.Application.Models;

public sealed class CreateFeedbackRequest
{
    public string CustomerId { get; set; } = string.Empty;
    public string? Comment { get; set; }
}