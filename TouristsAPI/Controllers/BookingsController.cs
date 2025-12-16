using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TouristsAPI.ErrorResponses;
using TouristsAPI.Helpers;
using TouristsCore.DTOS.Booking;
using TouristsCore.Services;

namespace TouristsAPI.Controllers;
[ApiController]
[Route("api/[controller]")]
[EnableRateLimiting("Global")]
public class BookingsController : ControllerBase
{
    
    private readonly IBookingService _bookingService;

    public BookingsController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }
    
    [HttpPost]
    [Authorize(Roles = "Tourist")]
    public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        try
        {
            var result = await _bookingService.CreateBookingAsync(dto, Guid.Parse(userId));
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiErrorResponse(400, ex.Message));
        }
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Tourist")]
    public async Task<IActionResult> CancelBooking(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        try
        {
            await _bookingService.CancelBookingAsync(id, Guid.Parse(userId));
            return Ok(new { Message = "Booking cancelled successfully. Seats have been returned." });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiErrorResponse(400, ex.Message));
        }
    }

    [HttpGet("my-bookings")]
    [Authorize(Roles = "Tourist")]
    public async Task<IActionResult> GetMyBookings([FromQuery] PaginationArg arg)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        try
        {
            var (result, count) = await _bookingService.GetBookingsForUserAsync(Guid.Parse(userId), arg);
        
            return Ok(new Pagination<BookingTouristDto>
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
    
    [HttpGet("sales/{tourId:int}")]
    [Authorize(Roles = "Guide")]
    public async Task<IActionResult> GetTourSales(int tourId, [FromQuery] PaginationArg arg)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        try
        {
            var (result, count) = await _bookingService.GetSalesForTourAsync(tourId, Guid.Parse(userId), arg);
        
            return Ok(new Pagination<GuideSalesDto>
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
    
    [HttpGet("{id:int}")]
    [Authorize] 
    public async Task<IActionResult> GetBookingById(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        try
        {
            var result = await _bookingService.GetBookingByIdAsync(id, Guid.Parse(userId));
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiErrorResponse(400, ex.Message));
        }
    }
    
    
}