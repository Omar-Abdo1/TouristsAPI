using Microsoft.AspNetCore.Identity;
using TouristsCore.Entities;

namespace TouristsCore.Services;

public interface ITokenService
{
    Task<string> CreateTokenAsync(User user, UserManager<User> userManager);
}