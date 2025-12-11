using TouristsCore.DTOS.Schedule;

namespace TouristsCore.Services;

public interface ITourScheduleService
{
    Task<ScheduleResponseDto> CreateScheduleAsync(int tourId, CreateScheduleDto dto, Guid userId);
    Task<ScheduleResponseDto> UpdateScheduleAsync(int scheduleId, UpdateScheduleDto dto, Guid userId);
    Task DeleteScheduleAsync(int scheduleId, Guid userId);
}