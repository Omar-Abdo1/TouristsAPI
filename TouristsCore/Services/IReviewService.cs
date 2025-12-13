using TouristsAPI.Helpers;
using TouristsCore.DTOS.Reviews;
using TouristsCore.Entities;

namespace TouristsCore.Services;

public interface IReviewService
{
    Task<int>CreateReviewAsync(CreateReviewDto dto,Guid userId);
    Task UpdateReviewAsync(int id,UpdateReviewDto dto, Guid userId);
    Task DeleteReviewAsync(int id,Guid userId);
    Task<TourReviewsDto>GetReviewForTourAsync(int tourId,PaginationArg arg); 
}