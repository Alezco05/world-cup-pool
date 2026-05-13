using Microsoft.EntityFrameworkCore;
using WorldCupPool.Api.Application.Services.Interfaces;
using WorldCupPool.Api.Domain;
using WorldCupPool.Api.Infrastructure;

namespace WorldCupPool.Api.Application.Services
{
    public class MatchService : IMatchService
    {
        private readonly ApplicationDbContext _context;

        public MatchService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Match>> GetMatchesAsync()
        {
            return await _context.Matches
                .OrderBy(m => m.MatchDate)
                .ToListAsync();
        }
    }
}
