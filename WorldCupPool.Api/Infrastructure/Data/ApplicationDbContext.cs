using Microsoft.EntityFrameworkCore;
using WorldCupPool.Api.Domain;

namespace WorldCupPool.Api.Infrastructure
{
    // DbContext principal para la aplicación, con configuración de entidades y relaciones.
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Conjuntos de entidades que EF Core mapea a tablas.
        public DbSet<User> Users => Set<User>();
        public DbSet<Match> Matches => Set<Match>();
        public DbSet<Prediction> Predictions => Set<Prediction>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de la entidad User.
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.PasswordHash)
                    .IsRequired();

                entity.Property(e => e.Role)
                    .HasConversion<string>()
                    .IsRequired();
            });

            // Configuración de la entidad Match.
            modelBuilder.Entity<Match>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.HomeTeam)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.AwayTeam)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Group)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.MatchDate)
                    .IsRequired();

                entity.Property(e => e.Status)
                    .HasConversion<string>()
                    .IsRequired();
            });

            // Configuración de la entidad Prediction.
            modelBuilder.Entity<Prediction>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.PredictedHomeScore)
                    .IsRequired();

                entity.Property(e => e.PredictedAwayScore)
                    .IsRequired();

                entity.Property(e => e.CreatedAt)
                    .IsRequired();

                entity.Property(e => e.PointsEarned);

                // Índice único compuesto para evitar predicciones duplicadas del mismo usuario para un partido.
                entity.HasIndex(e => new { e.UserId, e.MatchId })
                    .IsUnique();

                // Relación entre Prediction y User.
                entity.HasOne(e => e.User)
                    .WithMany(u => u.Predictions)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Relación entre Prediction y Match.
                entity.HasOne(e => e.Match)
                    .WithMany(m => m.Predictions)
                    .HasForeignKey(e => e.MatchId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
