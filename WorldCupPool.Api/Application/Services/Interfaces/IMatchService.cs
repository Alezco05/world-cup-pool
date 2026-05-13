using WorldCupPool.Api.Domain;

namespace WorldCupPool.Api.Application.Services.Interfaces
{
    public interface IMatchService
    {
        Task<IEnumerable<Match>> GetMatchesAsync();
    }
}
