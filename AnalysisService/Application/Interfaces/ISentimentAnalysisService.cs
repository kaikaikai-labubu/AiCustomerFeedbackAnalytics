using AnalysisService.Application.Models;

namespace AnalysisService.Application.Interfaces
{
    public interface ISentimentAnalysisService
    {
        Task<SentimentResult> AnalyzeAsync(string feedback);
    }
}
