using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using InventarioSystem.Core.DTOs;
using InventarioSystem.Core.Entities;
using InventarioSystem.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace InventarioSystem.Core.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<ServiceResponse<AuthResponseDto>> RegisterAsync(RegisterDto registerDto)
        {
            if (await _userRepository.EmailExistsAsync(registerDto.Email))
            {
                return ServiceResponse<AuthResponseDto>.FailureResponse("El email ya está registrado");
            }

            var user = new User
            {
                Username = registerDto.Username,
                Email = registerDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                Role = registerDto.Role,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            var createdUser = await _userRepository.CreateAsync(user);

            var token = GenerateJwtToken(createdUser.Id, createdUser.Email, createdUser.Role);

            var response = new AuthResponseDto
            {
                UserId = createdUser.Id,
                Username = createdUser.Username,
                Email = createdUser.Email,
                Role = createdUser.Role,
                Token = token
            };

            return ServiceResponse<AuthResponseDto>.SuccessResponse(response, "Usuario registrado exitosamente");
        }

        public async Task<ServiceResponse<AuthResponseDto>> LoginAsync(LoginDto loginDto)
        {
            var user = await _userRepository.GetByEmailAsync(loginDto.Email);

            if (user == null)
            {
                return ServiceResponse<AuthResponseDto>.FailureResponse("Credenciales inválidas");
            }

            if (!user.IsActive)
            {
                return ServiceResponse<AuthResponseDto>.FailureResponse("Usuario desactivado");
            }

            if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                return ServiceResponse<AuthResponseDto>.FailureResponse("Credenciales inválidas");
            }

            var token = GenerateJwtToken(user.Id, user.Email, user.Role);

            var response = new AuthResponseDto
            {
                UserId = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                Token = token
            };

            return ServiceResponse<AuthResponseDto>.SuccessResponse(response, "Inicio de sesión exitoso");
        }

        public Task<ServiceResponse<bool>> ValidateTokenAsync(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]!);

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["JwtSettings:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["JwtSettings:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out _);

                return Task.FromResult(ServiceResponse<bool>.SuccessResponse(true, "Token válido"));
            }
            catch
            {
                return Task.FromResult(ServiceResponse<bool>.FailureResponse("Token inválido"));
            }
        }

        public string GenerateJwtToken(int userId, string email, string role)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var expirationMinutes = int.Parse(_configuration["JwtSettings:ExpirationMinutes"] ?? "60");

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
