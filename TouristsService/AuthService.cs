using System.IdentityModel.Tokens.Jwt;
using Google.Apis.Auth;
using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
    private readonly IEmailService _emailService;
    private readonly IBackgroundJobClient _jobClient;
    private readonly ILogger<AuthService> _logger;

    public AuthService(UserManager<User> userManager,IConfiguration  configuration,TouristsContext  touristsContext,
        ITokenService tokenService,IEmailService  emailService,IBackgroundJobClient jobClient,ILogger<AuthService>  logger)
    {
        _userManager = userManager;
        _configuration = configuration;
        _touristsContext = touristsContext;
        _tokenService = tokenService;
        _emailService = emailService;
        _jobClient = jobClient;
        _logger = logger;
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
            await _touristsContext.GuideProfiles.AddAsync(new GuideProfile() {UserId   = user.Id,Bio = "",FullName = model.Username});
        }
        else
        {
            await _touristsContext.TouristProfiles.AddAsync(new TouristProfile() {UserId = user.Id , FullName = model.Username});
        }
        await _touristsContext.SaveChangesAsync();
        
        try 
        {
            var subject = "Welcome to TourApp! 🌍";
            var body = $@"
            <div style='font-family: Arial, sans-serif; padding: 20px;'>
                <h2 style='color: #2c3e50;'>Welcome, {model.Username}!</h2>
                <p>We are thrilled to have you on board.</p>
                <p>You can now log in and start booking your next adventure.</p>
                <br>
                <p>Best Regards,<br>The TourApp Team</p>
            </div>";
            _jobClient.Enqueue(()=> _emailService.SendEmailAsync(user.Email, subject, body));
        }
        catch(Exception ex) 
        {
            _logger.LogError(ex,$"Failed to Send Email for {user.Email}");
        }
        
        
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
        
        if(user!=null && !user.IsActive)
            return new AuthResponseDto() { Message = "Account is Disabled" };
            

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

            var result = await _userManager.CreateAsync(user, Guid.NewGuid().ToString()+"Abc@123"); // Random Password
            
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
    
    public async Task ForgotPasswordAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null) 
            throw new Exception("If that email exists, we have sent a reset link."); // Security: Don't reveal if user exists

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        
        var body = $@"
            <h1>Reset Your Password</h1>
            <p>You requested a password reset. Use the token below:</p>
            <p><strong>{token}</strong></p>
            <p>If you did not request this, ignore this email.</p>";

        _jobClient.Enqueue(()=>_emailService.SendEmailAsync(user.Email, "Reset Password Request", body));
    }

    public async Task ResetPasswordAsync(ResetPasswordDto model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user == null) throw new Exception("Invalid request.");

        var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new Exception(errors);
        }
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

        var roles = await _userManager.GetRolesAsync(user);
        string finalRole = roles.Contains("Guide") ? "Guide" : "Tourist";

        return new AuthResponseDto
        {
            IsAuthenticated = true,
            Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
            ExpiresOn = jwtToken.ValidTo,
            RefreshToken = refreshToken.Token,
            RefreshTokenExpiration = refreshToken.ExpiresOn,
            Role = finalRole,
            Username = user.UserName,
            Email = user.Email  
        };
    }
}