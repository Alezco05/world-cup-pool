using System;

namespace WorldCupPool.Api.Domain
{
    // Entidad que representa una predicción de un usuario para un partido.
    public class Prediction
    {
        public int Id { get; set; }

        // FK hacia el usuario que hace la predicción.
        public int UserId { get; set; }

        // FK hacia el partido que se predice.
        public int MatchId { get; set; }

        // Goles predichos para el equipo local.
        public int PredictedHomeScore { get; set; }

        // Goles predichos para el equipo visitante.
        public int PredictedAwayScore { get; set; }

        // Puntos ganados por esta predicción, puede ser nulo si aún no se calculó.
        public int? PointsEarned { get; set; }

        // Fecha de creación de la predicción.
        public DateTime CreatedAt { get; set; }

        // Navegación al usuario que hizo la predicción.
        public User User { get; set; } = null!;

        // Navegación al partido predicho.
        public Match Match { get; set; } = null!;
    }
}
