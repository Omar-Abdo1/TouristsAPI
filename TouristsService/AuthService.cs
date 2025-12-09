using System.IdentityModel.Tokens.Jwt;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TouristsCore.DTOS.Accounts;
using TouristsCore.Entities;
using TouristsCore.Services;
using TouristsRepository;

namespace TouristsService;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _configuration;
    private readonly TouristsContext _touristsContext;
    private readonly ITokenService _tokenService;

    public AuthService(UserManager<User> userManager,IConfiguration  configuration,TouristsContext  touristsContext,ITokenService tokenService)
    {
        _userManager = userManager;
        _configuration = configuration;
        _touristsContext = touristsContext;
        _tokenService = tokenService;
    }
    
    public async Task<AuthResponseDto> RegisterAsync(RegisterDto model)
    {
        if(await _userManager.FindByEmailAsync(model.Email) is not null)
            return new AuthResponseDto(){Message = "Email already exists"};

        var user = new User()
        {
            UserName = model.Username,
            Email = model.Email,
            PhoneNumber = model.PhoneNumber,
            IsActive = true
        };
        var res = await _userManager.CreateAsync(user, model.Password);
        if(!res.Succeeded)
            return new AuthResponseDto(){Message = string.Join(", ", res.Errors.Select(x => x.Description))};
        await _userManager.AddToRoleAsync(user, model.Role);
        if (model.Role.ToUpper() == "GUIDE")
        {
            await _touristsContext.GuideProfiles.AddAsync(new GuideProfile() {UserId   = user.Id,Bio = ""});
        }
        else
        {
            await _touristsContext.TouristProfiles.AddAsync(new TouristProfile() {UserId = user.Id , FullName = model.Username});
        }
        await _touristsContext.SaveChangesAsync();
        return await CreateAuthResponse(user);
    }
    
    public async Task<AuthResponseDto> LoginWithGoogleAsync(SocialLoginDto model)
    {
        GoogleJsonWebSignature.Payload payload;
        
        try
        {
            //todo You need a Client ID from the Google Cloud Console.
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new List<string> { _configuration["Google:ClientId"] }
            };
            payload = await GoogleJsonWebSignature.ValidateAsync(model.Token, settings);
            // Validate the Token from Frontend
        }
        catch
        {
            return new AuthResponseDto { Message = "Invalid Google Token" };
        }
        
        //check if Google user is already logged in before with GoogleId
        // Step A
        var user = await _userManager.FindByLoginAsync("Google",payload.Subject);

        if (user != null)
        {
            await _touristsContext.Entry(user).Collection(u => u.RefreshTokens).LoadAsync();
            return await CreateAuthResponse(user);
        }
        
        // see the Email  Step B
        user = await _userManager.FindByEmailAsync(payload.Email);
        
        // create Shadow Account
        if (user is null)
        {
            user = new User
            {
                UserName = payload.Email.Split('@')[0],
                Email = payload.Email,
                IsActive = true
            };

            var result = await _userManager.CreateAsync(user, Guid.NewGuid().ToString()); // Random Password
            
            if (!result.Succeeded) 
                return new AuthResponseDto { Message = "Could not register Google user." };

            await _userManager.AddToRoleAsync(user, model.Role);
            
            // Create Profile
            if (model.Role.ToUpper() == "GUIDE")
                await _touristsContext.GuideProfiles.AddAsync(new GuideProfile { UserId = user.Id, Bio = "Joined via Google" });
            else
                await _touristsContext.TouristProfiles.AddAsync(new TouristProfile { UserId = user.Id, FullName = payload.Name, Country = "Unknown" });
            
            await _touristsContext.SaveChangesAsync();
        }
        await _userManager.AddLoginAsync(user,
            new UserLoginInfo("Google", payload.Subject, "Google")); 
        // Link this user with GoogleId so next time we find by it in Step A
        return await CreateAuthResponse(user);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if(user is null || !await _userManager.CheckPasswordAsync(user, model.Password))
            return new AuthResponseDto(){Message = "Invalid Email Or Password"};
        if (!user.IsActive)
            return new AuthResponseDto() { Message = "Account is Disabled" };

        await _touristsContext.Entry(user)
            .Collection(u => u.RefreshTokens)
            .LoadAsync(); // load tokens from DB
        
        return await CreateAuthResponse(user);
    }
    
    public async Task<AuthResponseDto> RefreshTokenAsync(string token)
    {
         
        var user = _touristsContext.Users.Include(u=>u.RefreshTokens)
            .SingleOrDefault(u=>u.RefreshTokens.Any(t => t.Token == token));
        // get me the user who has this token
        if(user is null)
            return new AuthResponseDto(){Message = "Invalid Token"};
        var refreshToken = user.RefreshTokens.SingleOrDefault(t => t.Token == token);

        if (!refreshToken.IsActive)
            return new AuthResponseDto() { Message = "Inactive Token" };
        
        refreshToken.RevokedOn = DateTime.UtcNow;
        return await CreateAuthResponse(user); // here we update the user so also update its refreshToken 
    }

    public async Task<bool> RevokeTokenAsync(string token)
    {
        var user = _touristsContext.Users.Include(u=>u.RefreshTokens)
            .SingleOrDefault(u=>u.RefreshTokens.Any(t => t.Token == token));
        if (user is null)
            return false;
        var refreshToken = user.RefreshTokens.SingleOrDefault(t => t.Token == token);
        if (!refreshToken.IsActive)
            return false;
        refreshToken.RevokedOn = DateTime.UtcNow;
        await _touristsContext.SaveChangesAsync();
        return true;
    }
    private async Task<AuthResponseDto> CreateAuthResponse(User user)
    {
        var jwtToken = await _tokenService.CreateTokenAsync(user,_userManager);
        var refreshToken =  _tokenService.GenerateRefreshToken();

        // Remove old inactive tokens to keep DB clean
        var expiredTokens = user.RefreshTokens.
            Where(t => t.IsExpired || t.RevokedOn != null)
            .ToList();

        foreach (var oldToken in expiredTokens)
            user.RefreshTokens.Remove(oldToken);
        
        user.RefreshTokens.Add(refreshToken);
        await _touristsContext.SaveChangesAsync();

        return new AuthResponseDto
        {
            IsAuthenticated = true,
            Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
            ExpiresOn = jwtToken.ValidTo,
            RefreshToken = refreshToken.Token,
            RefreshTokenExpiration = refreshToken.ExpiresOn,
            Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault(),
            Username = user.UserName,
            Email = user.Email  
        };
    }
}