using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using TouristsAPI.ErrorResponses;
using TouristsCore.DTOS.Accounts;
using TouristsCore.Services;

namespace TouristsAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly IProfileService _profileService;

    public ProfileController(IProfileService  profileService)
    {
        _profileService = profileService;
    }
    
    [HttpGet("Me")]
    public async Task<IActionResult> GetMyProfile()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) 
                     ?? User.FindFirstValue("sub"); 

        if (userId == null)
            return Unauthorized(new ApiErrorResponse(401));

        var profile = await _profileService.GetUserProfileAsync(userId);

        if (profile is null)
            return NotFound(new ApiErrorResponse(404, "User not found"));

        return Ok(profile);
    }

    [HttpPatch("avatar")]
    public async Task<IActionResult> ChangeAvatar([FromBody]ChangeAvatarDto avatar)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var res = await _profileService.ChangeAvatarAsync(Guid.Parse(userId), avatar.FileId);
        if(!res)
            return NotFound(new ApiErrorResponse(404, "Avatar not found"));
        return Ok(new { message = "Avatar updated successfully" });
    }
    
    [HttpPut("tourist")]
    public async Task<IActionResult> UpdateTouristProfile(TouristProfileUpdateDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _profileService.UpdateTouristProfileAsync(userId, dto);
            var res = await _profileService.GetUserProfileAsync(userId);
            return Ok(res);
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiErrorResponse(400,ex.Message));
        }
    }
    [Authorize(Roles = "Guide")] 
    [HttpPut("guide")]
    public async Task<IActionResult> UpdateGuideProfile(GuideProfileUpdateDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _profileService.UpdateGuideProfileAsync(userId, dto);
            var res = await _profileService.GetUserProfileAsync(userId);
            return Ok(res);
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiErrorResponse(400,ex.Message));
        }
    }
    [HttpPost("become-guide")]
    public async Task<IActionResult> BecomeGuide()
    {
        var userId = GetCurrentUserId();
        
        var (success, message) = await _profileService.BecomeGuideAsync(userId);

        if (!success) return BadRequest(message);

        return Ok(new { message });
    }
    
    [AllowAnonymous]
    [HttpGet("guide/{userId}")]
    public async Task<IActionResult> GetGuidePublicProfile(string userId)
    {
        var result = await _profileService.GetGuidePublicProfileAsync(userId);

        if (result == null) return NotFound("Guide not found or profile incomplete.");

        return Ok(result);
    }
    
    
    public class ChangeAvatarDto
    {
        public int FileId { get; set; }
    }
    private string GetCurrentUserId()=>User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
}