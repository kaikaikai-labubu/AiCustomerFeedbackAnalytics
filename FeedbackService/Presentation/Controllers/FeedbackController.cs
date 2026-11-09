using FeedbackService.Application.Interfaces;
using FeedbackService.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace FeedbackService.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class FeedbackController(IFeedbackService feedbackService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var feedbackItems = await feedbackService.GetAllAsync(cancellationToken);
        return Ok(feedbackItems);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var feedback = await feedbackService.GetByIdAsync(id, cancellationToken);
        return feedback is null ? NotFound() : Ok(feedback);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateFeedbackRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.CustomerId))
        {
            return BadRequest("CustomerId is required.");
        }

        var created = await feedbackService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }
}