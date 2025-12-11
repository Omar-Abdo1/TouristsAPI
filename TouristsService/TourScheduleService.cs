using System.Formats.Asn1;
using Microsoft.EntityFrameworkCore;
using TouristsAPI.Helpers;
using TouristsCore;
using TouristsCore.DTOS.Schedule;
using TouristsCore.Entities;
using TouristsCore.Enums;
using TouristsCore.Services;

namespace TouristsService;

public class TourScheduleService : ITourScheduleService
{
    private readonly IUnitOfWork _unitOfWork;

    public TourScheduleService(IUnitOfWork  unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<ScheduleResponseDto> CreateScheduleAsync(int tourId, CreateScheduleDto dto, Guid userId)
    {
        var tour = await _unitOfWork.Repository<Tour>().GetByIdAsync(tourId,true,t=>t.GuideProfile);
        if(tour == null)
            throw new Exception($"Tour With {tourId}  not found");
        if(tour.GuideProfile.UserId!= userId)
            throw new Exception("You are not authorized to add schedules to this tour.");
        if(dto.StartTime<DateTime.UtcNow)
            throw new Exception("You cannot create a schedule in the past.");
        var schdule = new TourSchedule()
        {
            TourId = tourId,
            StartTime = dto.StartTime,
            AvailableSeats = dto.Capacity
        };
        _unitOfWork.Repository<TourSchedule>().Add(schdule);
        await _unitOfWork.CompleteAsync();
        return MaptoDto(schdule);
    }


    public async Task<ScheduleResponseDto> UpdateScheduleAsync(int scheduleId, UpdateScheduleDto dto, Guid userId)
    {
        var schedule = await _unitOfWork.Repository<TourSchedule>().GetByIdAsync(scheduleId, false, 
            s => s.Bookings,s=>s.Tour.GuideProfile);
        if(schedule == null)
            throw new Exception($"schedule With {scheduleId}  not found");
        if(schedule.Tour.GuideProfile.UserId!= userId)
            throw new Exception("You are not authorized to update schedules to this tour.");
        
        if (dto.StartTime.HasValue)
        {
            if (dto.StartTime.Value < DateTime.UtcNow)
                throw new Exception("Cannot move schedule to the past.");
            schedule.StartTime = dto.StartTime.Value;
        }
        
        if (dto.Capacity.HasValue)
        {
            int ticketsSold= schedule.Bookings.Where(b => b.Status != BookingStatus.Cancelled).Sum(b => b.TicketCount);
            
            if(ticketsSold>dto.Capacity.Value)
                throw new Exception($"Cannot reduce capacity to {dto.Capacity.Value}. You have already sold {ticketsSold} tickets.");
            schedule.AvailableSeats = dto.Capacity.Value - ticketsSold;
        }
        _unitOfWork.Repository<TourSchedule>().Update(schedule);
        await _unitOfWork.CompleteAsync();
        return MaptoDto(schedule);
    }

    public async Task DeleteScheduleAsync(int scheduleId, Guid userId)
    {
        var schedule = await _unitOfWork.Repository<TourSchedule>().GetByIdAsync(scheduleId, false,
            s => s.Bookings,s=>s.Tour.GuideProfile);
        if(schedule == null)
            throw new Exception($"schedule With {scheduleId}  not found");
        if(schedule.Tour.GuideProfile.UserId!= userId)
            throw new Exception("You are not authorized to update schedules to this tour.");
        if (schedule.Bookings.Any(b=>b.Status!=BookingStatus.Cancelled))
        {
            throw new Exception("Cannot delete this schedule because tourists have already booked tickets. Please cancel their bookings first.");
        }

        _unitOfWork.Repository<TourSchedule>().DeletePermanently(schedule); 
        await _unitOfWork.CompleteAsync();
    }

    public async Task<(IReadOnlyList<ScheduleResponseDto>, int)> GetSchedulesForTourAsync(int tourId, bool isGuide,PaginationArg arg)
    {
        var query = _unitOfWork.Context.Set<TourSchedule>()
            .AsQueryable()
            .AsNoTracking()
            .Where(s => s.TourId == tourId); 

        if (!isGuide)
            query = query.Where(s => s.StartTime > DateTime.UtcNow && s.AvailableSeats > 0);
        
        int totalCount = await query.CountAsync();

        var schedules = await query
            .Skip((arg.PageIndex - 1) * arg.PageSize)
            .Take(arg.PageSize)
            .Select(s => new ScheduleResponseDto()
            {
                Id = s.Id,
                TourId = s.TourId,
                StartTime = s.StartTime,
                AvailableSeats = s.AvailableSeats
            })
            .ToListAsync();

        return (schedules, totalCount);
    }
     
    public async Task<ScheduleResponseDto> GetScheduleByIdAsync(int scheduleId)
    {
        var schedule = await _unitOfWork.Repository<TourSchedule>()
            .GetByIdAsync(scheduleId);

        if (schedule == null)
            throw new Exception($"Schedule with id {scheduleId} not found");

        return MaptoDto(schedule);
    } 
    
    private ScheduleResponseDto MaptoDto(TourSchedule schdule)
    {
        return new ScheduleResponseDto()
        {
            AvailableSeats = schdule.AvailableSeats,
            TourId = schdule.TourId,
            StartTime = schdule.StartTime,
            Id = schdule.Id,
        };
    }
}