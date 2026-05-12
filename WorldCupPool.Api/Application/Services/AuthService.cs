using Microsoft.EntityFrameworkCore;
using WorldCupPool.Api.Application.DTOs;
using WorldCupPool.Api.Domain;
using WorldCupPool.Api.Infrastructure;
using WorldCupPool.Api.Infrastructure.Auth;

namespace WorldCupPool.Api.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly ITokenService _tokenService;

        public AuthService(ApplicationDbContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
        {
            // Validaciones de datos básicas
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (string.IsNullOrWhiteSpace(request.Username))
                throw new InvalidOperationException("El usuario es obligatorio.");

            if (string.IsNullOrWhiteSpace(request.Email))
                throw new InvalidOperationException("El correo electrónico es obligatorio.");

            if (string.IsNullOrWhiteSpace(request.Password))
                throw new InvalidOperationException("La contraseña es obligatoria.");

            if (request.Password.Length < 6)
                throw new InvalidOperationException("La contraseña debe tener al menos 6 caracteres.");

            // Normalizar email (case-insensitive)
            var normalizedEmail = request.Email.Trim().ToLowerInvariant();

            // Verificar que el email no esté registrado
            var existingUser = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email.ToLower() == normalizedEmail);

            if (existingUser != null)
                throw new InvalidOperationException("El correo electrónico ya está registrado.");

            // Crear nuevo usuario
            var user = new User
            {
                Username = request.Username.Trim(),
                Email = normalizedEmail,
                PasswordHash = PasswordHasher.HashPassword(request.Password),
                Role = UserRole.User
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var token = _tokenService.GenerateToken(user);
            return new AuthResponseDto
            {
                Token = token,
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role.ToString()
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
        {
            // Validaciones de datos básicas
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (string.IsNullOrWhiteSpace(request.Email))
                throw new InvalidOperationException("El correo electrónico es obligatorio.");

            if (string.IsNullOrWhiteSpace(request.Password))
                throw new InvalidOperationException("La contraseña es obligatoria.");

            // Normalizar email (case-insensitive)
            var normalizedEmail = request.Email.Trim().ToLowerInvariant();

            // Buscar usuario por email (case-insensitive)
            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email.ToLower() == normalizedEmail);

            if (user == null || !PasswordHasher.VerifyPassword(request.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Credenciales inválidas. Verifique el correo y contraseña.");

            var token = _tokenService.GenerateToken(user);
            return new AuthResponseDto
            {
                Token = token,
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role.ToString()
            };
        }
    }
}
