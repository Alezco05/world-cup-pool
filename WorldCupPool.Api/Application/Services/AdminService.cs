using Microsoft.EntityFrameworkCore;
using WorldCupPool.Api.Application.DTOs;
using WorldCupPool.Api.Domain;
using WorldCupPool.Api.Infrastructure;

namespace WorldCupPool.Api.Application.Services
{
    public class AdminService : IAdminService
    {
        private readonly ApplicationDbContext _context;
        private readonly IPredictionService _predictionService;

        public AdminService(ApplicationDbContext context, IPredictionService predictionService)
        {
            _context = context;
            _predictionService = predictionService;
        }

        public async Task<AdminMatchScoreUpdateResponse> UpdateMatchScoreAsync(int matchId, MatchScoreUpdateRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var match = await _context.Matches.FirstOrDefaultAsync(m => m.Id == matchId);
            if (match == null)
                throw new KeyNotFoundException("El partido no existe.");

            // Validación estricta: solo se puede actualizar si está en estado Scheduled
            if (match.Status != MatchStatus.Scheduled)
                throw new InvalidOperationException(
                    $"No se puede actualizar el marcador. El partido está en estado '{match.Status}'. " +
                    $"Solo se permiten actualizaciones en partidos con estado 'Scheduled' para evitar corrupción de datos.");

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Actualizar el partido
                match.HomeScore = request.HomeScore;
                match.AwayScore = request.AwayScore;
                match.Status = MatchStatus.Finished;

                // Obtener todas las predicciones para este partido
                var predictions = await _context.Predictions
                    .Where(p => p.MatchId == matchId)
                    .ToListAsync();

                // Calcular puntos para cada predicción
                foreach (var prediction in predictions)
                {
                    prediction.PointsEarned = _predictionService.CalculatePoints(
                        request.HomeScore,
                        request.AwayScore,
                        prediction.PredictedHomeScore,
                        prediction.PredictedAwayScore);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new AdminMatchScoreUpdateResponse
                {
                    MatchId = match.Id,
                    HomeScore = match.HomeScore.GetValueOrDefault(),
                    AwayScore = match.AwayScore.GetValueOrDefault(),
                    Status = match.Status.ToString(),
                    PredictionsProcessed = predictions.Count
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
