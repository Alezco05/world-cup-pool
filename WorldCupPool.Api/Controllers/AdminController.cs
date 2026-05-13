using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorldCupPool.Api.Application.DTOs;
using WorldCupPool.Api.Application.Services;

namespace WorldCupPool.Api.Controllers
{
    /// <summary>
    /// Controlador delgado para operaciones administrativas.
    /// Requiere rol de Admin. Delega toda la lógica a IAdminService.
    /// </summary>
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        /// <summary>
        /// Actualiza el resultado de un partido y recalcula puntos de predicciones.
        /// Solo permite actualizar partidos en estado 'Scheduled'.
        /// </summary>
        [HttpPost("matches/{id}/score")]
        public async Task<IActionResult> UpdateMatchScore(int id, [FromBody] MatchScoreUpdateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _adminService.UpdateMatchScoreAsync(id, request);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
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

        /// <summary>
        /// Abre o reabre de forma forzada cualquier partido en cualquier fecha (Pasada o Futura).
        /// Borra marcadores oficiales anteriores y limpia los puntos calculados de los usuarios.
        /// </summary>
        [HttpPost("matches/{id}/toggle-open")]
        public async Task<IActionResult> ForceOpenMatch(int id)
        {
            try
            {
                var result = await _adminService.ForceOpenMatchAsync(id);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
