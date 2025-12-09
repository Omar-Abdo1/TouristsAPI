using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using TouristsAPI.ErrorResponses;
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
    
    public class ChangeAvatarDto
    {
        public int FileId { get; set; }
    }
}