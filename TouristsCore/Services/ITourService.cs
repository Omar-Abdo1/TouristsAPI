using TouristsCore.DTOS.Tours;

namespace TouristsCore.Services;

public interface ITourService
{
    Task<int> CreateTourAsync(CreateTourDto model, Guid userId);
    Task<TourDto>GetTourByIdAsync(int id);
    Task<(IReadOnlyList<TourDto>,int)>GetToursAsync(TourRequestDto request);

    Task<bool> DeleteTourAsync(int tourId,Guid userId);
    Task<TourDto>UpdateTourAsync(int tourId,Guid userId,UpdateTourDto model);
}