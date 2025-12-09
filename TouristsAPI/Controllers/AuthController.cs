using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TouristsAPI.ErrorResponses;
using TouristsCore.DTOS.Accounts;
using TouristsCore.Entities;
using TouristsCore.Services;

namespace TouristsAPI.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly UserManager<User> _userManager;
    private readonly ITokenService _tokenService;

    public AuthController(IAuthService  authService,UserManager<User>  userManager,ITokenService tokenService)
    {
        _authService = authService;
        _userManager = userManager;
        _tokenService = tokenService;
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

    [HttpPost("forget-password")]
    public async Task<ActionResult<string>> ForgotPassword(ForgotPasswordDto model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if(user == null)
            return NotFound(new ApiErrorResponse(404,"User not found"));
        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        //todo will send email 
        return Ok(token);
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordDto model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if(user == null)
            return NotFound(new ApiErrorResponse(404,"User not found"));
        var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword); // check if the token is valid for this user 
        if(!result.Succeeded)
        return BadRequest(result.Errors);
        return Ok(new
        {
          Displayname = user.UserName,
          Email = user.Email,
          Token = new JwtSecurityTokenHandler().
              WriteToken(await _tokenService.CreateTokenAsync(user,_userManager)),
        });
    }
    
    public class TokenRequestDto
    {
        public string Token { get; set; }
    }
}