using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorldCupPool.Api.Application.DTOs;
using WorldCupPool.Api.Application.Services;

namespace WorldCupPool.Api.Controllers
{
    [ApiController]
    [Route("api/predictions")]
    [Authorize]
    public class PredictionsController : ControllerBase
    {
        private readonly IPredictionService _predictionService;

        public PredictionsController(IPredictionService predictionService)
        {
            _predictionService = predictionService;
        }

        [HttpPost]
        public async Task<IActionResult> PostPrediction([FromBody] PredictionRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetAuthenticatedUserId();
            if (userId is null)
                return Unauthorized("No se pudo obtener el usuario autenticado.");

            try
            {
                var response = await _predictionService.CreateOrUpdatePredictionAsync(userId.Value, request);
                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetHistory()
        {
            var userId = GetAuthenticatedUserId();
            if (userId is null)
                return Unauthorized("No se pudo obtener el usuario autenticado.");

            try
            {
                var history = await _predictionService.GetHistoryAsync(userId.Value);
                return Ok(history);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al recuperar el historial: {ex.Message}");
            }
        }

        /// <summary>
        /// Extrae el ID del usuario del token JWT utilizando el claim estándar ClaimTypes.NameIdentifier.
        /// </summary>
        private int? GetAuthenticatedUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(userIdClaim, out var userId) ? userId : null;
        }
    }
}
