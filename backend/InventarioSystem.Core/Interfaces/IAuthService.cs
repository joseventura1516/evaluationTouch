using InventarioSystem.Core.DTOs;

namespace InventarioSystem.Core.Interfaces
{
    public interface IAuthService
    {
        Task<ServiceResponse<AuthResponseDto>> RegisterAsync(RegisterDto registerDto);
        Task<ServiceResponse<AuthResponseDto>> LoginAsync(LoginDto loginDto);
        Task<ServiceResponse<bool>> ValidateTokenAsync(string token);
        string GenerateJwtToken(int userId, string email, string role);
    }
}
