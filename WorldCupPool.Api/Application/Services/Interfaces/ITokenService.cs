namespace WorldCupPool.Api.Application.Services
{
    public interface ITokenService
    {
        string GenerateToken(Domain.User user);
    }
}
