using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorldCupPool.Api.Application.Services;

namespace WorldCupPool.Api.Controllers
{
    /// <summary>
    /// Controlador delgado para consultas de clasificación y ranking.
    /// Delega toda la lógica de negocio a ILeaderboardService.
    /// </summary>
    [ApiController]
    [Route("api/leaderboard")]
    [Authorize]
    public class LeaderboardController : ControllerBase
    {
        private readonly ILeaderboardService _leaderboardService;

        public LeaderboardController(ILeaderboardService leaderboardService)
        {
            _leaderboardService = leaderboardService;
        }

        /// <summary>
        /// Retorna el ranking global de usuarios ordenado por puntos totales.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetLeaderboard()
        {
            try
            {
                var leaderboard = await _leaderboardService.GetLeaderboardAsync();
                return Ok(leaderboard);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al recuperar el ranking: {ex.Message}");
            }
        }

        /// <summary>
        /// Retorna el historial de predicciones de un usuario específico.
        /// </summary>
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserHistory(int userId)
        {
            try
            {
                var history = await _leaderboardService.GetUserHistoryAsync(userId);
                return Ok(history);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al recuperar historial: {ex.Message}");
            }
        }
    }
}
