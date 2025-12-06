using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Identity;
using TouristsCore.Entities;

namespace TouristsCore.Services;

public interface ITokenService
{
    Task<JwtSecurityToken> CreateTokenAsync(User user, UserManager<User> userManager);
    RefreshToken GenerateRefreshToken();
}