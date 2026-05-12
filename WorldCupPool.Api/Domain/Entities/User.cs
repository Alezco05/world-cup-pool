using System.Collections.Generic;

namespace WorldCupPool.Api.Domain
{
    // Roles permitidos para un usuario de la aplicación.
    public enum UserRole
    {
        User,
        Admin
    }

    // Entidad que representa a un usuario de la polla mundialista.
    public class User
    {
        public int Id { get; set; }

        // Nombre de usuario para autenticación y visualización.
        public string Username { get; set; } = null!;

        // Correo electrónico del usuario.
        public string Email { get; set; } = null!;

        // Hash de la contraseña, nunca almacenar la contraseña en texto plano.
        public string PasswordHash { get; set; } = null!;

        // Rol del usuario dentro del sistema.
        public UserRole Role { get; set; } = UserRole.User;

        // Relación con las predicciones que ha hecho el usuario.
        public ICollection<Prediction> Predictions { get; set; } = new List<Prediction>();
    }
}
