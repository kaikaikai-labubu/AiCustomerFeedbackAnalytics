namespace FeedbackService.Application.Models;

public sealed class CreateFeedbackRequest
{
    public string CustomerId { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string? Comment { get; set; }
}