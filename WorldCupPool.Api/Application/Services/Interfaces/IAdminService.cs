using WorldCupPool.Api.Application.DTOs;

namespace WorldCupPool.Api.Application.Services
{
    public interface IAdminService
    {
        Task<AdminMatchScoreUpdateResponse> UpdateMatchScoreAsync(int matchId, MatchScoreUpdateRequest request);
        Task<AdminMatchScoreUpdateResponse> ForceOpenMatchAsync(int matchId);
    }
}
