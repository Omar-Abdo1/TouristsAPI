using Microsoft.AspNetCore.Mvc;
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
    
}