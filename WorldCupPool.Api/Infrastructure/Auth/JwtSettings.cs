namespace WorldCupPool.Api.Infrastructure
{
    public sealed record JwtSettings(string Key, string Issuer, string Audience, int ExpirationMinutes);
}
