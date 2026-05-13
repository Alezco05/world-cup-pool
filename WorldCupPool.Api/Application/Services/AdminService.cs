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

        /// <summary>
        /// ELIMINAMOS LA RESTRICCIÓN: Ahora permite guardar y corregir marcadores las veces que quieras.
        /// </summary>
        public async Task<AdminMatchScoreUpdateResponse> UpdateMatchScoreAsync(int matchId, MatchScoreUpdateRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var match = await _context.Matches.FirstOrDefaultAsync(m => m.Id == matchId);
            if (match == null)
                throw new KeyNotFoundException("El partido no existe.");

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Guardamos el nuevo marcador oficial
                match.HomeScore = request.HomeScore;
                match.AwayScore = request.AwayScore;
                match.Status = MatchStatus.Finished; // Cambia o mantiene el estado en Finished

                // Buscamos todas las predicciones de los usuarios para este encuentro
                var predictions = await _context.Predictions
                    .Where(p => p.MatchId == matchId)
                    .ToListAsync();

                // Recalculamos los puntos de cada usuario con el nuevo resultado (sobreescribe los anteriores)
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

        /// <summary>
        /// Este método limpia todo a cero y reabre el partido sin importar su estado previo.
        /// </summary>
        public async Task<AdminMatchScoreUpdateResponse> ForceOpenMatchAsync(int matchId)
        {
            var match = await _context.Matches.FirstOrDefaultAsync(m => m.Id == matchId);
            if (match == null)
                throw new KeyNotFoundException("El partido no existe.");

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Forzamos el estado a Live (En Curso) utilizando tu Enum
                match.Status = MatchStatus.Live;

                // Inicializamos el marcador en 0-0 para que empiece la edición en vivo
                match.HomeScore = 0;
                match.AwayScore = 0;

                // Reseteamos a cero los puntos calculados de los usuarios para este partido
                var predictions = await _context.Predictions
                    .Where(p => p.MatchId == matchId)
                    .ToListAsync();

                foreach (var prediction in predictions)
                {
                    prediction.PointsEarned = 0;
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new AdminMatchScoreUpdateResponse
                {
                    MatchId = match.Id,
                    HomeScore = 0,
                    AwayScore = 0,
                    Status = match.Status.ToString(), // Retorna "Live" de forma exacta en el JSON
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
