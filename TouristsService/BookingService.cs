using Microsoft.EntityFrameworkCore;
using TouristsCore;
using TouristsCore.DTOS.Booking;
using TouristsCore.Entities;
using TouristsCore.Enums;
using TouristsCore.Services;

namespace TouristsService;

public class BookingService : IBookingService
{
    private readonly IUnitOfWork _unitOfWork;
    private const int MaxRetries = 5;
    public BookingService(IUnitOfWork  unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task<BookingResponseDto> CreateBookingAsync(CreateBookingDto dto, Guid userId)
    {
        int maxRetries = MaxRetries;
        int currentRetry = 0;
        while (currentRetry < maxRetries)
        {
            try
            {
                var schedule = await _unitOfWork.Repository<TourSchedule>().GetByIdAsync(dto.ScheduleId, false,
                    s => s.Tour);
                if (schedule == null)
                    throw new Exception($"Schedule with id = {dto.ScheduleId} not found.");
                
                var touristProfile = await _unitOfWork.Repository<TouristProfile>()
                    .GetEntityByConditionAsync(t => t.UserId == userId);
                
                if (touristProfile == null) throw new Exception("Tourist profile not found");
                
                if(schedule.AvailableSeats<dto.TicketCount)
                    throw new Exception($"Sold out! Only {schedule.AvailableSeats} seats remaining.");
                
                schedule.AvailableSeats-=dto.TicketCount;
                
                _unitOfWork.Repository<TourSchedule>().Update(schedule); // now it will read TimeStamp
                
                var booking = new Booking()
                {
                    TouristId = touristProfile.Id,
                    TourScheduleId = dto.ScheduleId,
                    TourId = schedule.TourId,
                    TicketCount = dto.TicketCount,
                    PriceAtBooking = schedule.Tour.Price * dto.TicketCount,
                    Status = BookingStatus.Confirmed,
                    BookingDate = DateTime.UtcNow
                };
                _unitOfWork.Repository<Booking>().Add(booking);
                await _unitOfWork.CompleteAsync(); // If RowVersion changed in DB since we read it, this throws exception
                
                return new BookingResponseDto
                {
                    BookingId = booking.Id,
                    TourName = schedule.Tour.Title,
                    TourDate = schedule.StartTime,
                    Tickets = booking.TicketCount,
                    TotalPrice = booking.PriceAtBooking,
                    Status = booking.Status.ToString()
                };
                
            }
            catch (DbUpdateConcurrencyException) // SQL update lock the row that being saved so no one can make anything while another thread is saving but both can read concurrently
            {
                // Someone else bought the ticket milliseconds before us.
                currentRetry++;
                // Clear memory
                _unitOfWork.Context.ChangeTracker.Clear();

                await Task.Delay(50 * currentRetry); 
            }
        }
        throw new Exception("High demand! Someone else booked these seats first. Please try again.");
    }

    

    public async Task CancelBookingAsync(int bookingId, Guid userId)
    {
        int maxRetries = MaxRetries;
        int currentRetry = 0;
        while (currentRetry < maxRetries)
        {
            try
            {
                var booking = await _unitOfWork.Repository<Booking>().GetByIdAsync(bookingId, false,b=>b.TourSchedule);
                
                if (booking == null)
                    throw new Exception("Booking not found.");
                var touristProfile = await _unitOfWork.Repository<TouristProfile>()
                    .GetEntityByConditionAsync(t => t.UserId == userId,true);
                if (booking.TouristId != touristProfile.Id)
                    throw new Exception("You are not authorized to cancel this booking.");
                if (booking.Status == BookingStatus.Cancelled)
                    throw new Exception("This booking is already cancelled.");

                if (booking.TourSchedule.StartTime < DateTime.UtcNow.AddHours(24)) // business Logic 
                throw new Exception("Cannot cancel within 24 hours of the tour.");

                booking.Status = BookingStatus.Cancelled;
                booking.TourSchedule.AvailableSeats += booking.TicketCount;
                
                _unitOfWork.Repository<TourSchedule>().Update(booking.TourSchedule);
                _unitOfWork.Repository<Booking>().Update(booking);

                await _unitOfWork.CompleteAsync();
                return;
            }
            catch (DbUpdateConcurrencyException)
            {
                currentRetry++;
                _unitOfWork.Context.ChangeTracker.Clear();
                await Task.Delay(50 * currentRetry);
            }
        }
        throw new Exception("System is busy. Please try cancelling again.");
    }
}