using WorldCupPool.Api.Application.DTOs;

namespace WorldCupPool.Api.Application.Services
{
    public interface ILeaderboardService
    {
        Task<IEnumerable<LeaderboardEntryDto>> GetLeaderboardAsync();
        Task<IEnumerable<UserPredictionHistoryEntryDto>> GetUserHistoryAsync(int userId);
    }
}