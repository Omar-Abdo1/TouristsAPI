using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TouristsAPI.ErrorResponses;
using TouristsAPI.Helpers;
using TouristsCore.DTOS.Tours;
using TouristsCore.Entities;
using TouristsCore.Services;

namespace TouristsAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[EnableRateLimiting("Global")]
public class TourController : ControllerBase
{
    private readonly ITourService _tourService;

    public TourController(ITourService  tourService)
    {
        _tourService = tourService;
    }

    [HttpPost]
    [Authorize(Roles = "Guide")]
    public async Task<IActionResult> CreateTour([FromBody]CreateTourDto model)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        try
        {
            var tourId = await _tourService.CreateTourAsync(model, Guid.Parse(userId));
            return CreatedAtRoute(nameof(GetTourById), new { id = tourId }, new { id = tourId, message = "Tour created successfully" });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiErrorResponse(400, ex.Message));
        }
    }
    
    

    [HttpGet("{id:int}", Name = "GetTourById")]
    public async Task<IActionResult> GetTourById(int id)
    {
        var tour = await _tourService.GetTourByIdAsync(id);
        if (tour == null)
            return NotFound(new ApiErrorResponse(404, $"No Tour With id : {id}"));
        return Ok(tour);
    }
    
   
    [HttpGet]
    public async Task<IActionResult> GetTours([FromQuery]TourRequestDto request)
    {
         var (tours, count) = await _tourService.GetToursAsync(request);
         return Ok(new Pagination<TourDto>()
         {
             Count = count,
             PageIndex = request.PageNumber,
             PageSize = request.PageSize,
             Data = tours
         });
    }
    

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Guide")]
    public async Task<IActionResult> UpdateTour(int id, [FromBody] UpdateTourDto model)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        try
        {
            var updateTour = await _tourService.UpdateTourAsync(id, Guid.Parse(userId), model);
            if (updateTour == null)
                return NotFound(new ApiErrorResponse(404, "Tour Not Found"));
            return Ok(updateTour);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }
    

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Guide")]
    public async Task<IActionResult> DeleteTour(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        try
        {
            var success = await _tourService.DeleteTourAsync(id, Guid.Parse(userId));
            if (!success)
                return NotFound(new ApiErrorResponse(404, "Tour Not Found"));
            return Ok(new { message = "Tour deleted successfully" });        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }
    
    [Authorize(Roles = "Guide")] 
    [HttpGet("my-tours")]
    public async Task<IActionResult> GetMyTours([FromQuery] PaginationArg arg)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var (tours,count) = await _tourService.GetMyToursAsync(userId, arg);
            
            return Ok(
                new Pagination<TourDto>()
                {
                    PageIndex = arg.PageIndex,
                    PageSize = arg.PageSize,
                    Count = count,
                    Data = tours
                }
                );
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }
    
    [Authorize(Roles = "Guide")]
    [HttpPatch("{id:int}/publish")]
    public async Task<IActionResult> TogglePublish(int id)
    {
        try 
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var newStatus = await _tourService.TogglePublishStatusAsync(id, userId);
            
            return Ok(new 
            { 
                Message = newStatus ? "Tour is now LIVE" : "Tour is now HIDDEN", 
                IsPublished = newStatus 
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ApiErrorResponse(404, ex.Message));
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new ApiErrorResponse(403, ex.Message));
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiErrorResponse(400, ex.Message));
        }
    }
    
}