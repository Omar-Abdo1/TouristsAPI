using Microsoft.EntityFrameworkCore;
using TouristsAPI.Helpers;
using TouristsCore;
using TouristsCore.DTOS.Reviews;
using TouristsCore.Entities;
using TouristsCore.Enums;
using TouristsCore.Services;

namespace TouristsService;

public class ReviewService : IReviewService
{
    private readonly IUnitOfWork _unitOfWork;

    public ReviewService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task<int> CreateReviewAsync(CreateReviewDto dto, Guid userId)
    {
        var tourist = await _unitOfWork.Repository<TouristProfile>().GetEntityByConditionAsync(
            t => t.UserId == userId, true);
        if (tourist == null)
            throw new Exception("Tourist not found");
        var booking = await _unitOfWork.Repository<Booking>().GetByIdAsync(
            dto.BookingId, true, b => b.TourSchedule);
        if (booking == null)
            throw new Exception("Booking not found");
        
        if(booking.TouristId!=tourist.Id)
            throw new Exception("You can only review your own bookings.");
        
        if(booking.Status==BookingStatus.Cancelled)
            throw new Exception("You cannot review a cancelled trip.");
        
        if (booking.TourSchedule.StartTime > DateTime.UtcNow) 
            throw new Exception("You cannot review a tour that hasn't happened yet.");

        var existingReview = await _unitOfWork.Repository<Review>()
            .GetEntityByConditionAsync(r => r.BookingId == dto.BookingId, true)
            ;
        if (existingReview != null)
            throw new Exception("You have already reviewed this specific trip.");

        var review = new Review()
        {
            BookingId = dto.BookingId,
            TouristId = tourist.Id,
            TourId = booking.TourId,
            Comment = dto.Comment,
            Rating = dto.Rating,
        };
        
        _unitOfWork.Repository<Review>().Add(review);
        await _unitOfWork.CompleteAsync();
        return review.Id;
    }

    public async Task UpdateReviewAsync(int id,UpdateReviewDto dto, Guid userId)
    {
        var review = await _unitOfWork.Repository<Review>()
            .GetByIdAsync(id);

        if (review == null) throw new Exception("Review not found.");
        
        var tourist = await _unitOfWork.Repository<TouristProfile>()
            .GetEntityByConditionAsync(t => t.UserId == userId);
        
        if (tourist == null)
            throw new Exception("Tourist not found");
        
        if (review.TouristId != tourist.Id)
            throw new Exception("You can only edit your own reviews.");
        
        review.Rating = dto.Rating;
        review.Comment = dto.Comment;
        review.UpdatedAt = DateTime.UtcNow;
        
        _unitOfWork.Repository<Review>().Update(review);
        await _unitOfWork.CompleteAsync();
    }

    public async Task DeleteReviewAsync(int id, Guid userId)
    {
        var review = await _unitOfWork.Repository<Review>()
            .GetByIdAsync(id);
        
        if (review == null) throw new Exception("Review not found.");
        
        var tourist = await _unitOfWork.Repository<TouristProfile>()
            .GetEntityByConditionAsync(t => t.UserId == userId);
        
        if (tourist == null)
            throw new Exception("Tourist not found");
        
        if (review.TouristId != tourist.Id)
            throw new Exception("You can only delete your own reviews.");
        
        _unitOfWork.Repository<Review>().SoftDelete(review);
        await _unitOfWork.CompleteAsync();
    }

    public async Task<TourReviewsDto> GetReviewForTourAsync(int tourId, PaginationArg arg)
    {
       
        var query = _unitOfWork.Context.Set<Review>().AsQueryable()
            .AsNoTracking()
            .Where(r => r.TourId == tourId);

        var status = await query
            .GroupBy(r => 1)
            .Select(r => new
            {
                Average = r.Average(r => r.Rating),
                Count = r.Count()
            })
            .FirstOrDefaultAsync();

        var reviews = await query
            .OrderByDescending(r => r.CreatedAt)
            .Skip((arg.PageIndex - 1) * arg.PageSize)
            .Take(arg.PageSize)
            .Select(r => new ReviewResponseDto()
            {
              Id  = r.Id,
              TouristName = r.Tourist.FullName!=null ?  r.Tourist.FullName : "UnKnown",
              Rating = r.Rating,
              Comment = r.Comment!=null ? r.Comment : "No Comment",
                Date = r.CreatedAt
            })
            .ToListAsync();

        return new TourReviewsDto()
        {
            AverageRating = status?.Average?? 0,
            TotalReviews = status?.Count?? 0 ,
            Reviews = reviews
        };

    }
}