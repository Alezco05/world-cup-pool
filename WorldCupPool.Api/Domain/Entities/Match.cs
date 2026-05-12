using System;
using System.Collections.Generic;

namespace WorldCupPool.Api.Domain
{
    // Estado del partido dentro de la competición.
    public enum MatchStatus
    {
        Scheduled,
        Finished
    }

    // Entidad que representa un partido de la copa mundial.
    public class Match
    {
        public int Id { get; set; }

        // Equipo local.
        public string HomeTeam { get; set; } = null!;

        // Equipo visitante.
        public string AwayTeam { get; set; } = null!;

        // Grupo de la fase de grupos.
        public string Group { get; set; } = null!;

        // Fecha y hora del partido.
        public DateTime MatchDate { get; set; }

        // Goles anotados por el equipo local (nulo si aún no se jugó).
        public int? HomeScore { get; set; }

        // Goles anotados por el equipo visitante (nulo si aún no se jugó).
        public int? AwayScore { get; set; }

        // Estado actual del partido.
        public MatchStatus Status { get; set; } = MatchStatus.Scheduled;

        // Relación con las predicciones realizadas para este partido.
        public ICollection<Prediction> Predictions { get; set; } = new List<Prediction>();
    }
}
