using Microsoft.EntityFrameworkCore;
using WorldCupPool.Api.Application.DTOs;
using WorldCupPool.Api.Domain;
using WorldCupPool.Api.Infrastructure;

namespace WorldCupPool.Api.Application.Services
{
    public class PredictionService : IPredictionService
    {
        private readonly ApplicationDbContext _context;

        public PredictionService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Calcula puntos según el resultado predicho vs real usando Math.Sign para determinar tendencias.
        /// </summary>
        public int CalculatePoints(int realHome, int realAway, int predHome, int predAway)
        {
            // Exact score: 3 points
            if (realHome == predHome && realAway == predAway)
                return 3;

            var realDiff = realHome - realAway;
            var predDiff = predHome - predAway;

            // Exact draw (both 0): 1 point
            if (realDiff == 0 && predDiff == 0)
                return 1;

            // Same trend (both positive, both negative, or both zero): 1 point
            if (Math.Sign(realDiff) == Math.Sign(predDiff) && predDiff != 0)
                return 1;

            // No match: 0 points
            return 0;
        }

        public async Task<PredictionHistoryResponse> CreateOrUpdatePredictionAsync(int userId, PredictionRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            // Obtener el partido
            var match = await _context.Matches
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == request.MatchId);

            if (match == null)
                throw new InvalidOperationException("El partido no existe.");

            // Validación de reloj inquebrantable: no se puede predecir después de la fecha del partido
            if (DateTime.UtcNow >= match.MatchDate)
                throw new InvalidOperationException(
                    $"No se puede crear o actualizar la predicción. El tiempo para predecir este partido ha expirado " +
                    $"(fecha del partido: {match.MatchDate:O}).");

            if (match.Status != MatchStatus.Scheduled)
                throw new InvalidOperationException("Solo se pueden crear o actualizar predicciones para partidos programados.");

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var prediction = await _context.Predictions
                    .FirstOrDefaultAsync(p => p.MatchId == request.MatchId && p.UserId == userId);

                if (prediction == null)
                {
                    prediction = new Prediction
                    {
                        UserId = userId,
                        MatchId = request.MatchId,
                        PredictedHomeScore = request.PredictedHomeScore,
                        PredictedAwayScore = request.PredictedAwayScore,
                        CreatedAt = DateTime.UtcNow
                    };

                    _context.Predictions.Add(prediction);
                }
                else
                {
                    prediction.PredictedHomeScore = request.PredictedHomeScore;
                    prediction.PredictedAwayScore = request.PredictedAwayScore;
                    prediction.PointsEarned = null; // Reset points when updating
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new PredictionHistoryResponse
                {
                    PredictionId = prediction.Id,
                    MatchId = prediction.MatchId,
                    HomeTeam = match.HomeTeam,
                    AwayTeam = match.AwayTeam,
                    PredictedHomeScore = prediction.PredictedHomeScore,
                    PredictedAwayScore = prediction.PredictedAwayScore,
                    HomeScore = match.HomeScore,
                    AwayScore = match.AwayScore,
                    PointsEarned = prediction.PointsEarned,
                    Status = match.Status.ToString()
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<IEnumerable<PredictionHistoryResponse>> GetHistoryAsync(int userId)
        {
            return await _context.Predictions
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .Include(p => p.Match)
                .Select(p => new PredictionHistoryResponse
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
                    Status = p.Match.Status.ToString()
                })
                .ToListAsync();
        }
    }
}
