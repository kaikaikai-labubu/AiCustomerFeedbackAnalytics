using AnalysisService.Application.Interfaces;
using AnalysisService.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace AnalysisService.Presentation
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class AnalysisController(IAnalysisRepository analysisRepository) : ControllerBase
    {
        [HttpGet("feedback-analyses")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFeedbackAnalysesByFilter([FromQuery] GetFeedbackRequest request, CancellationToken cancellationToken)
        {
            var analyses = await analysisRepository.GetFeedbackAnalysisByFiltersAsync(request, cancellationToken);
            return Ok(analyses);
        }
    }
}
