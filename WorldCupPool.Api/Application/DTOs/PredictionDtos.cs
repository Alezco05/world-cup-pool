using System.ComponentModel.DataAnnotations;

namespace WorldCupPool.Api.Application.DTOs
{
    public sealed class PredictionRequest
    {
        [Required(ErrorMessage = "El ID del partido es obligatorio.")]
        public int MatchId { get; set; }

        [Required(ErrorMessage = "El marcador predicho local es obligatorio.")]
        [Range(0, 20, ErrorMessage = "El marcador local debe estar entre 0 y 20 goles.")]
        public int PredictedHomeScore { get; set; }

        [Required(ErrorMessage = "El marcador predicho visitante es obligatorio.")]
        [Range(0, 20, ErrorMessage = "El marcador visitante debe estar entre 0 y 20 goles.")]
        public int PredictedAwayScore { get; set; }
    }

    public sealed class PredictionHistoryResponse
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
    }

    public sealed class MatchScoreUpdateRequest
    {
        [Required(ErrorMessage = "El marcador local es obligatorio.")]
        [Range(0, 20, ErrorMessage = "El marcador local debe estar entre 0 y 20 goles.")]
        public int HomeScore { get; set; }

        [Required(ErrorMessage = "El marcador visitante es obligatorio.")]
        [Range(0, 20, ErrorMessage = "El marcador visitante debe estar entre 0 y 20 goles.")]
        public int AwayScore { get; set; }
    }
}

