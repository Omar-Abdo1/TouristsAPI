using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TouristsAPI.ErrorResponses;
using TouristsAPI.Helpers;
using TouristsCore.DTOS.Reviews;
using TouristsCore.Services;

namespace TouristsAPI.Controllers;
[ApiController]
[Route("api/[controller]")]
public class ReviewController : ControllerBase
{
    private readonly IReviewService _reviewService;

    public ReviewController(IReviewService  reviewService)
    {
        _reviewService = reviewService;
    }

    [HttpPost]
    [Authorize(Roles = "Tourist")]
    public async Task<IActionResult> CreateReview(CreateReviewDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        try
        {
            var id = await _reviewService.CreateReviewAsync(dto, Guid.Parse(userId));
            return Ok(new
            {
                Message = "Review Created Successfully",
                Id = id
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiErrorResponse(400, ex.Message));
        }
    }
    
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Tourist")]
    public async Task<IActionResult> DeleteReview(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        try
        {
             await _reviewService.DeleteReviewAsync(id, Guid.Parse(userId));
            return Ok(new
            {
                Message = "Review Deleted Successfully",
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiErrorResponse(400, ex.Message));
        }
    }
    
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Tourist")]
    public async Task<IActionResult> UpdateReview(int id, UpdateReviewDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        try
        {
            await _reviewService.UpdateReviewAsync(id, dto,Guid.Parse(userId));
            return Ok(new
            {
                Message = "Review Updated Successfully"
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiErrorResponse(400, ex.Message));
        }
    }
    
    [HttpGet("{tourId:int}")]
    public async Task<IActionResult> GetReviews(int tourId,[FromQuery]PaginationArg arg)
    {
        try
        {
            var res = await _reviewService.GetReviewForTourAsync(tourId,arg);
            return Ok(res);
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiErrorResponse(400, ex.Message));
        }
    }
    
}























