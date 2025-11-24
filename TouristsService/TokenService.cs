using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TouristsCore.Entities;
using TouristsCore.Services;

namespace TouristsService;

public class TokenService : ITokenService
{
    private readonly IConfiguration _config;

    public TokenService(IConfiguration  config)
    {
        _config = config;
    }

    public async Task<string> CreateTokenAsync(User user, UserManager<User> userManager)
    {
        
        // Fill the PayLoad

        var securityStamp = await userManager.GetSecurityStampAsync(user);

        var userClaims = new List<Claim>()
        {
            // predefined Claims : 
            new Claim(JwtRegisteredClaimNames.Email,  user.Email??string.Empty),
            new Claim(ClaimTypes.MobilePhone, user.PhoneNumber??string.Empty),
            new Claim(ClaimTypes.Name,user.UserName??string.Empty),
            new Claim(JwtRegisteredClaimNames.Sub ,user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()) ,
            // Token Generated ID so it changes every Time

            // custom Claims : 
        };
        var Roles = await userManager.GetRolesAsync(user);
        foreach (var role in Roles)
            userClaims.Add(new Claim(ClaimTypes.Role, role));

        
        var AuthKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["JWT:SecretKey"]));

        var Token = new JwtSecurityToken(
            issuer: _config["JWT:IssuerIP"],
            audience:_config["JWT:AudienceIP"],
            expires:DateTime.UtcNow.AddMinutes(
                double.Parse(_config["JWT:DurationInMinutes"])) ,
            claims:userClaims,
            signingCredentials: new SigningCredentials(AuthKey , SecurityAlgorithms.HmacSha256Signature)
        );
        return new JwtSecurityTokenHandler().WriteToken(Token);
    }
    
    public RefreshToken GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return new RefreshToken
        {
            Token = Convert.ToBase64String(randomNumber),
            ExpiresOn = DateTime.UtcNow.AddDays(10),
            CreatedOn = DateTime.UtcNow
        };
    }
}