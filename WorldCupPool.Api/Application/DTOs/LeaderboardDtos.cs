namespace WorldCupPool.Api.Application.DTOs
{
    public sealed class LeaderboardEntryDto
    {
        public int UserId { get; set; }
        public string Username { get; set; } = null!;
        public int TotalPoints { get; set; }
    }

    public sealed class UserPredictionHistoryEntryDto
    {
        public int PredictionId { get; set; }
        public int MatchId { get; set; }
        public string HomeTeam { get; set; } = null!;
        public string AwayTeam { get; set; } = null!;
        public int PredictedHomeScore { get; set; }
        public int PredictedAwayScore { get; set; }
        public int? HomeScore { get; set; }
        public int? AwayScore { get; set; }
        public int? PointsEarned { get; set; }
        public string Status { get; set; } = null!;
        public DateTime CreatedAt { get; set; }

        public DateTime MatchDate { get; set; }
    }
}