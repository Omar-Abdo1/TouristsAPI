using TouristsAPI.Helpers;
using TouristsCore.DTOS.Schedule;

namespace TouristsCore.Services;

public interface ITourScheduleService
{
    Task<ScheduleResponseDto> CreateScheduleAsync(int tourId, CreateScheduleDto dto, Guid userId);
    Task<ScheduleResponseDto> UpdateScheduleAsync(int scheduleId, UpdateScheduleDto dto, Guid userId);
    Task DeleteScheduleAsync(int scheduleId, Guid userId);
    
    Task<(IReadOnlyList<ScheduleResponseDto>,int)>GetSchedulesForTourAsync(int tourId,bool isGuide,PaginationArg arg);

    Task<ScheduleResponseDto> GetScheduleByIdAsync(int scheduleId);
    
}