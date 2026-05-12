namespace WorldCupPool.Api.Application.DTOs
{
    public sealed class AdminMatchScoreUpdateResponse
    {
        public int MatchId { get; set; }
        public int HomeScore { get; set; }
        public int AwayScore { get; set; }
        public string Status { get; set; } = null!;
        public int PredictionsProcessed { get; set; }
    }
}
