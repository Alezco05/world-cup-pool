using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorldCupPool.Api.Application.Services.Interfaces;

namespace WorldCupPool.Api.Controllers
{
    [ApiController]
    [Route("api/matches")]
    [Authorize]
    public class MatchesController : ControllerBase
    {
        private readonly IMatchService _matchService;

        public MatchesController(IMatchService matchService)
        {
            _matchService = matchService;
        }

        [HttpGet]
        public async Task<IActionResult> GetMatches()
        {
            try
            {
                var matches = await _matchService.GetMatchesAsync();
                return Ok(matches);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al recuperar los partidos: {ex.Message}");
            }
        }
    }
}
