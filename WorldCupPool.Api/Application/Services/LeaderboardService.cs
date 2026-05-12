using Microsoft.EntityFrameworkCore;
using WorldCupPool.Api.Application.DTOs;
using WorldCupPool.Api.Infrastructure;

namespace WorldCupPool.Api.Application.Services
{
    public class LeaderboardService : ILeaderboardService
    {
        private readonly ApplicationDbContext _context;

        public LeaderboardService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retorna el ranking global de usuarios ordenado por puntos totales acumulados.
        /// Usa la propiedad de navegación User.Username en GroupBy sin JOIN explícito.
        /// </summary>
        public async Task<IEnumerable<LeaderboardEntryDto>> GetLeaderboardAsync()
        {
            return await _context.Predictions
                .AsNoTracking()
                .Where(p => p.PointsEarned.HasValue)
                .GroupBy(p => new { p.UserId, p.User.Username })
                .Select(group => new LeaderboardEntryDto
                {
                    UserId = group.Key.UserId,
                    Username = group.Key.Username,
                    TotalPoints = group.Sum(p => p.PointsEarned ?? 0)
                })
                .OrderByDescending(x => x.TotalPoints)
                .ThenBy(x => x.Username)
                .ToListAsync();
        }

        /// <summary>
        /// Retorna el historial de predicciones del usuario con detalles del partido.
        /// Ordenado por fecha descendente.
        /// </summary>
        public async Task<IEnumerable<UserPredictionHistoryEntryDto>> GetUserHistoryAsync(int userId)
        {
            return await _context.Predictions
                .AsNoTracking()
                .Where(p => p.UserId == userId)
                .Include(p => p.Match)
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new UserPredictionHistoryEntryDto
                {
                    PredictionId = p.Id,
                    MatchId = p.MatchId,
                    HomeTeam = p.Match.HomeTeam,
                    AwayTeam = p.Match.AwayTeam,
                    PredictedHomeScore = p.PredictedHomeScore,
                    PredictedAwayScore = p.PredictedAwayScore,
                    HomeScore = p.Match.HomeScore,
                    AwayScore = p.Match.AwayScore,
                    PointsEarned = p.PointsEarned,
                    Status = p.Match.Status.ToString(),
                    CreatedAt = p.CreatedAt
                })
                .ToListAsync();
        }
    }
}
