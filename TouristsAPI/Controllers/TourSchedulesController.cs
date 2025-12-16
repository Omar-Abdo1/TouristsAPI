using System.Security.Claims;
using Bogus;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TouristsAPI.ErrorResponses;
using TouristsAPI.Helpers;
using TouristsCore.DTOS.Schedule;
using TouristsCore.Services;

namespace TouristsAPI.Controllers;
[ApiController]
[Route("api")]
[EnableRateLimiting("Global")]
public class TourSchedulesController : ControllerBase
{
    private readonly ITourScheduleService _tourScheduleService;

    public TourSchedulesController(ITourScheduleService  tourScheduleService )
    {
        _tourScheduleService = tourScheduleService;
    }
    [HttpPost("tours/{tourId:int}/schedules")]
    [Authorize(Roles = "Guide")]
    public async Task<IActionResult> CreateSchedule(int tourId, [FromBody] CreateScheduleDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        try
        {
            var result = await _tourScheduleService.CreateScheduleAsync(tourId, dto, Guid.Parse(userId));
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiErrorResponse(400,ex.Message));
        }
    }
    [HttpPut("schedules/{id:int}")]
    [Authorize(Roles = "Guide")]
    public async Task<IActionResult> UpdateSchedule(int id, [FromBody] UpdateScheduleDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        try
        {
            var result = await _tourScheduleService.UpdateScheduleAsync(id, dto, Guid.Parse(userId));
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiErrorResponse(400,ex.Message));
        }
    }

    [HttpDelete("schedules/{id:int}")]
    [Authorize(Roles = "Guide")]
    public async Task<IActionResult> DeleteSchedule(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        try
        {
             await _tourScheduleService.DeleteScheduleAsync(id, Guid.Parse(userId));
            return Ok(new
            {
                Message = "Deleted schedule Successfully."
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiErrorResponse(400, ex.Message));
        }
    }

    [HttpGet("tours/{tourId:int}/schedules")]
    public async Task<IActionResult> GetSchedules(int tourId,[FromQuery]PaginationArg arg)
    {
        var isGuide = User.IsInRole("Guide") || User.IsInRole("Admin");
    
        try
        {
            var (result,count) = await _tourScheduleService.GetSchedulesForTourAsync(tourId, isGuide,arg);
            return Ok(new Pagination<ScheduleResponseDto>()
            {
              Count = count,
              Data = result,
              PageIndex = arg.PageIndex,
              PageSize = arg.PageSize
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiErrorResponse(400, ex.Message));
        }
    }
    [HttpGet("schedules/{id:int}")]
    public async Task<IActionResult> GetScheduleById(int id)
    {
        try
        {
            var result = await _tourScheduleService.GetScheduleByIdAsync(id);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return NotFound(new ApiErrorResponse(404, ex.Message));
        }
    }


}