using System.Linq.Expressions;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TouristsAPI.Helpers;
using TouristsCore;
using TouristsCore.DTOS.Booking;
using TouristsCore.Entities;
using TouristsCore.Enums;
using TouristsCore.Services;

namespace TouristsService;

public class BookingService : IBookingService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;
    private readonly IBackgroundJobClient _jobClient;
    private readonly ILogger<BookingService> _logger;
    private const int MaxRetries = 5;
    public BookingService(IUnitOfWork  unitOfWork,IEmailService emailService,IBackgroundJobClient jobClient,ILogger<BookingService> logger)
    {
        _unitOfWork = unitOfWork;
        _emailService = emailService;
        _jobClient = jobClient;
        _logger = logger;
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
                    .GetEntityByConditionAsync(t => t.UserId == userId,false,
                        t=>t.User);
                
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
                    Status = BookingStatus.Pending,
                    BookingDate = DateTime.UtcNow
                };
                _unitOfWork.Repository<Booking>().Add(booking);
                await _unitOfWork.CompleteAsync(); // If RowVersion changed in DB since we read it, this throws exception
                
                
                try
                {
                    
                        var subject = "Booking Confirmation - Payment Required ⏳";
                        var body = $@"
                    <h2>Booking Received!</h2>
                    <p>Your booking for <strong>{booking.Tour.Title}</strong> is currently <strong>PENDING</strong>.</p>
                    <p style='color: red;'><strong>Important:</strong> You have 1 hour to complete the payment.</p>
                    <p>If payment is not received by {DateTime.UtcNow.AddHours(1):HH:mm} UTC, these seats will be automatically released.</p>
                    <br>
                    <a href='https://yourapp.com/pay/{booking.Id}'>Click here to Pay Now</a>";

                    _jobClient.Enqueue(()=>_emailService.SendEmailAsync(touristProfile.User.Email, subject, body));
                }
                catch(Exception ex) 
                {
                    _logger.LogError(ex,$"Failed to Send Email for {touristProfile.User.Email}");
                }
                
                
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
                var booking = await _unitOfWork.Repository<Booking>().GetByIdAsync(bookingId, false,
                    b=>b.TourSchedule,b=>b.Tour);
                
                if (booking == null)
                    throw new Exception("Booking not found.");
                var touristProfile = await _unitOfWork.Repository<TouristProfile>()
                    .GetEntityByConditionAsync(t => t.UserId == userId,true,t=>t.User);
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
                
                _jobClient.Enqueue(() => _emailService.SendEmailAsync(
                    touristProfile.User.Email, 
                    "Booking Cancelled", 
                    $"Your booking for {booking.Tour.Title} has been cancelled successfully."
                ));
                
                
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

    public async Task<(IReadOnlyList<BookingTouristDto>, int)> GetBookingsForUserAsync(Guid userId, PaginationArg arg)
    {
        var tourist = await _unitOfWork.Repository<TouristProfile>().GetEntityByConditionAsync(
            t => t.UserId == userId, true);
        if (tourist == null) throw new Exception("Tourist profile not found");

        var query = _unitOfWork.Context.Set<Booking>()
            .AsQueryable()
            .AsNoTracking()
            .Where(b => b.TouristId == tourist.Id);
        
        int count = await query.CountAsync();

        var bookings = await 
            query.OrderByDescending(b => b.BookingDate)
                .Skip((arg.PageIndex - 1) * arg.PageSize)
                .Take(arg.PageSize)
                .Select(b => new BookingTouristDto()
                {
                    
                    Id = b.Id,
                    TicketCount = b.TicketCount,
                    Price = b.PriceAtBooking,
                    Status = b.Status.ToString(),
                    BookingDate = b.BookingDate,
                    
                    TourName = b.Tour.Title,
                    City = b.Tour.City,
                    TourDate = b.TourSchedule.StartTime
                })
                .ToListAsync();
        return (bookings, count);
    }

    public async Task<(IReadOnlyList<GuideSalesDto>, int)> GetSalesForTourAsync(int tourId, Guid userId, PaginationArg arg)
    {
        var tour = await _unitOfWork.Repository<Tour>()
            .GetByIdAsync(tourId, true, t => t.GuideProfile);
        if (tour == null) 
            throw new Exception($"Tour with id = {tourId} not found");
        
        if(tour.GuideProfile.UserId!=userId)
            throw new Exception("Unauthorized. You do not own this tour.");

        var query = _unitOfWork.Context.Set<Booking>()
            .AsQueryable().AsNoTracking()
            .Where(b => b.TourId == tour.Id);

        int count = await query.CountAsync();
         
        var sales = await query
            .OrderByDescending(b => b.BookingDate)
            .Skip((arg.PageIndex - 1) * arg.PageSize)
            .Take(arg.PageSize)
            .Select(b => new GuideSalesDto()
            {
                BookingId = b.Id,
                TouristName = b.Tourist.FullName!=null ?  b.Tourist.FullName : "UnKnown",
                BookingDate = b.BookingDate,
                TourDate = b.TourSchedule.StartTime,
                TicketCount = b.TicketCount,
                TotalRevenue = b.PriceAtBooking,
                Status = b.Status.ToString()
            })
            .ToListAsync();
        return (sales, count);
    }

    public async Task<BookingDetailDto> GetBookingByIdAsync(int bookingId, Guid userId)
    {
        var booking = await _unitOfWork.Repository<Booking>().GetByIdAsync(
            bookingId, 
            true, 
            b => b.Tour.GuideProfile, 
            b => b.Tourist,
            b => b.TourSchedule
        );
        if(booking == null)
            throw new Exception($"Booking with id ={bookingId} not found");
        
        if(booking.Tour.GuideProfile.UserId != userId && booking.Tourist.UserId != userId)
            throw new Exception("Unauthorized. You cannot view this ticket.");

        return new BookingDetailDto()
            {
                Id = booking.Id,
                Status = booking.Status.ToString(),
                BookingDate = booking.BookingDate,

                TicketCount = booking.TicketCount,
                PricePaid = booking.PriceAtBooking,

                TourId = booking.TourId,
                TourName = booking.Tour.Title,
                TourDescription = booking.Tour.Description,
                City = booking.Tour.City,

                StartTime = booking.TourSchedule.StartTime,

                TouristName = booking.Tourist.FullName != null ? booking.Tourist.FullName : "UnKnown",
                GuideName = booking.Tour.GuideProfile.FullName != null ? booking.Tour.GuideProfile.FullName : "UnKnown",
            };
    }
}