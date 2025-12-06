using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TouristsAPI.ErrorResponses;
using TouristsCore.DTOS.Accounts;
using TouristsCore.Services;

namespace TouristsAPI.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService  authService)
    {
        _authService = authService;
    }

    [HttpPost("Register")]
    public async Task<IActionResult> RegisterAsync(RegisterDto model)
    {
        var res =await _authService.RegisterAsync(model);
        if (!res.IsAuthenticated)
            return BadRequest(new ApiErrorResponse(400, res.Message));
        return Ok(res);
    }
    
    [HttpPost("Login")]
    public async Task<IActionResult> LoginAsync( LoginDto model)
    {
        var res = await _authService.LoginAsync(model);
        if (!res.IsAuthenticated)
            return BadRequest(new ApiErrorResponse(400, res.Message));
        return Ok(res);
    }
    [HttpPost("Google-Login")]
    public async Task<IActionResult> GoogleLoginAsync(SocialLoginDto model)
    {
        var res = await _authService.LoginWithGoogleAsync(model);
        if (!res.IsAuthenticated)
            return BadRequest(new ApiErrorResponse(400, res.Message));
        return Ok(res);
    }
    
    [HttpPost("Refresh-Token")]
    public async Task<IActionResult> RefreshTokenAsync(TokenRequestDto model)
    {
        if (string.IsNullOrEmpty(model.Token))
            return BadRequest(new ApiErrorResponse(400,"Token Must Sent"));

        var result = await _authService.RefreshTokenAsync(model.Token);

        if (!result.IsAuthenticated)
            return BadRequest(new ApiErrorResponse(400, result.Message));

        return Ok(result);
    }
    
    [HttpPost("Revoke-Token")]
    public async Task<IActionResult> RevokeTokenAsync(TokenRequestDto model)
    {
        if (string.IsNullOrEmpty(model.Token))
            return BadRequest(new ApiErrorResponse(400,"Token Must Sent"));

        var result = await _authService.RevokeTokenAsync(model.Token);

        if (!result)
            return BadRequest(new ApiErrorResponse(400,"Token is invalid or inactive"));

        return Ok(new { message = "Token revoked successfully" });
    }
    
    [Authorize]
    [HttpGet("Me")]
    public async Task<IActionResult> GetMyProfile()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) 
                     ?? User.FindFirstValue("sub"); 

        if (userId == null)
            return Unauthorized(new ApiErrorResponse(401));

        var profile = await _authService.GetUserProfileAsync(userId);

        if (profile is null)
            return NotFound(new ApiErrorResponse(404, "User not found"));

        return Ok(profile);
    }
    
    
    public class TokenRequestDto
    {
        public string Token { get; set; }
    }
}