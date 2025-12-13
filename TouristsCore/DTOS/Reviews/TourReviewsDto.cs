namespace TouristsCore.DTOS.Reviews;

public class TourReviewsDto
{
    public double AverageRating { get; set; }
    public int TotalReviews { get; set; }
    public List<ReviewResponseDto> Reviews { get; set; } = new List<ReviewResponseDto>();
}