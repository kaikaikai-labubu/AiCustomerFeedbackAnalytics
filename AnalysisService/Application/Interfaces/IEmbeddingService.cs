using AnalysisService.Application.Models;

namespace AnalysisService.Application.Interfaces
{
    public interface IEmbeddingService
    {
        Task<ReadOnlyMemory<float>> GetEmbeddingAsync(string feedback);
    }
}
