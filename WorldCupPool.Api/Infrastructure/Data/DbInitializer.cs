using System;
using System.Linq;
using WorldCupPool.Api.Domain;
using WorldCupPool.Api.Infrastructure.Auth; // Asegúrate de que este sea el namespace de tu PasswordHasher

namespace WorldCupPool.Api.Infrastructure
{
    // Clase estática para sembrar datos iniciales en la base de datos.
    public static class DbInitializer
    {
        public static void Seed(ApplicationDbContext context)
        {
            // 1. Sembrar el usuario Administrador por defecto si la tabla de usuarios está vacía
            if (!context.Users.Any())
            {
                var adminUser = new User
                {
                    Username = "admin",
                    Email = "admin@polla.com",
                    // Se encripta la contraseña usando tu utilitario existente
                    PasswordHash = PasswordHasher.HashPassword("Admin1234*"), 
                    Role = UserRole.Admin // Usa tu Enum o string correspondiente (ej: UserRole.Admin o "Admin")
                };

                context.Users.Add(adminUser);
                context.SaveChanges();
            }

            // 2. Si ya existen partidos registrados, no se realiza ninguna semilla de fixture.
            if (context.Matches.Any())
            {
                return;
            }

            var utcNow = DateTime.UtcNow;

            var matches = new[]
            {
                // Grupo A
                new Match
                {
                    HomeTeam = "Argentina",
                    AwayTeam = "Mexico",
                    Group = "Grupo A",
                    MatchDate = utcNow.AddDays(1),
                    Status = MatchStatus.Scheduled,
                    HomeScore = null,
                    AwayScore = null
                },
                new Match
                {
                    HomeTeam = "Poland",
                    AwayTeam = "Saudi Arabia",
                    Group = "Grupo A",
                    MatchDate = utcNow.AddDays(2),
                    Status = MatchStatus.Scheduled,
                    HomeScore = null,
                    AwayScore = null
                },
                new Match
                {
                    HomeTeam = "Argentina",
                    AwayTeam = "Poland",
                    Group = "Grupo A",
                    MatchDate = utcNow.AddDays(4),
                    Status = MatchStatus.Scheduled,
                    HomeScore = null,
                    AwayScore = null
                },
                new Match
                {
                    HomeTeam = "Mexico",
                    AwayTeam = "Saudi Arabia",
                    Group = "Grupo A",
                    MatchDate = utcNow.AddDays(5),
                    Status = MatchStatus.Scheduled,
                    HomeScore = null,
                    AwayScore = null
                },
                new Match
                {
                    HomeTeam = "Saudi Arabia",
                    AwayTeam = "Argentina",
                    Group = "Grupo A",
                    MatchDate = utcNow.AddDays(7),
                    Status = MatchStatus.Scheduled,
                    HomeScore = null,
                    AwayScore = null
                },
                new Match
                {
                    HomeTeam = "Mexico",
                    AwayTeam = "Poland",
                    Group = "Grupo A",
                    MatchDate = utcNow.AddDays(8),
                    Status = MatchStatus.Scheduled,
                    HomeScore = null,
                    AwayScore = null
                },

                // Grupo B
                new Match
                {
                    HomeTeam = "France",
                    AwayTeam = "Australia",
                    Group = "Grupo B",
                    MatchDate = utcNow.AddDays(1),
                    Status = MatchStatus.Scheduled,
                    HomeScore = null,
                    AwayScore = null
                },
                new Match
                {
                    HomeTeam = "Denmark",
                    AwayTeam = "Tunisia",
                    Group = "Grupo B",
                    MatchDate = utcNow.AddDays(2),
                    Status = MatchStatus.Scheduled,
                    HomeScore = null,
                    AwayScore = null
                },
                new Match
                {
                    HomeTeam = "France",
                    AwayTeam = "Denmark",
                    Group = "Grupo B",
                    MatchDate = utcNow.AddDays(4),
                    Status = MatchStatus.Scheduled,
                    HomeScore = null,
                    AwayScore = null
                },
                new Match
                {
                    HomeTeam = "Australia",
                    AwayTeam = "Tunisia",
                    Group = "Grupo B",
                    MatchDate = utcNow.AddDays(5),
                    Status = MatchStatus.Scheduled,
                    HomeScore = null,
                    AwayScore = null
                },
                new Match
                {
                    HomeTeam = "Tunisia",
                    AwayTeam = "France",
                    Group = "Grupo B",
                    MatchDate = utcNow.AddDays(7),
                    Status = MatchStatus.Scheduled,
                    HomeScore = null,
                    AwayScore = null
                },
                new Match
                {
                    HomeTeam = "Australia",
                    AwayTeam = "Denmark",
                    Group = "Grupo B",
                    MatchDate = utcNow.AddDays(8),
                    Status = MatchStatus.Scheduled,
                    HomeScore = null,
                    AwayScore = null
                }
            };

            context.Matches.AddRange(matches);
            context.SaveChanges();
        }
    }
}
