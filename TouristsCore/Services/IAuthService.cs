using TouristsCore.DTOS.Accounts;

namespace TouristsCore.Services;

public interface IAuthService
{
    Task<AuthResponseDto>RegisterAsync(RegisterDto model);
    Task<AuthResponseDto>LoginAsync(LoginDto model);
    public Task<AuthResponseDto> LoginWithGoogleAsync(SocialLoginDto model);
   
    Task<AuthResponseDto> RefreshTokenAsync(string token);
    Task<bool> RevokeTokenAsync(string token);
}