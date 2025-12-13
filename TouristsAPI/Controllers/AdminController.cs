using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TouristsAPI.ErrorResponses;
using TouristsAPI.Helpers;
using TouristsCore.DTOS.Admin;
using TouristsCore.Services;

namespace TouristsAPI.Controllers;
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService  adminService)
    {
        _adminService = adminService;
    }
    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        try
        {
            var stats = await _adminService.GetSystemStatsAsync();
            return Ok(stats);
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiErrorResponse(400, ex.Message));
        }
    }
    
    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers([FromQuery] PaginationArg arg)
    {
        try 
        {
            var (users, count) = await _adminService.GetAllUsersAsync(arg);
            return Ok(new Pagination<AdminUserDto>
            {
                Data = users,
                Count = count,
                PageIndex = arg.PageIndex,
                PageSize = arg.PageSize
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiErrorResponse(400, ex.Message));
        }
    }
    
    [HttpPut("users/{id:guid}/toggle-ban")]
    public async Task<IActionResult> ToggleBan(Guid id)
    {
        try
        {
            await _adminService.ToggleUserBanAsync(id);
            return Ok(new { Message = "User status updated successfully." });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiErrorResponse(400, ex.Message));
        }
    }
    
}