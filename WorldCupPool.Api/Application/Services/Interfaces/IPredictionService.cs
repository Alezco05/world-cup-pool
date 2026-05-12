using WorldCupPool.Api.Application.DTOs;

namespace WorldCupPool.Api.Application.Services
{
    public interface IPredictionService
    {
        int CalculatePoints(int realHome, int realAway, int predHome, int predAway);
        Task<PredictionHistoryResponse> CreateOrUpdatePredictionAsync(int userId, PredictionRequest request);
        Task<IEnumerable<PredictionHistoryResponse>> GetHistoryAsync(int userId);
    }
}
