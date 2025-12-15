using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TouristsCore;
using TouristsCore.Entities;
using TouristsCore.Enums;
using TouristsCore.Enums.Payment;
using TouristsCore.Services;

namespace TouristsService;

public interface IJobService
{
    Task AutoCancelUnpaidBookings();
    Task DeleteOldFilesAsync();

    Task SendReviewRemindersAsync();
    
    Task CancelExpiredPaymentsAsync();
}

public class JobService : IJobService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _env;
    private readonly IEmailService _emailService;
    private readonly ILogger<JobService> _logger;

    public JobService(IUnitOfWork unitOfWork,IWebHostEnvironment env,IEmailService emailService,ILogger<JobService>  logger)
    {
        _unitOfWork = unitOfWork;
        _env = env;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task AutoCancelUnpaidBookings()
    {
        var oneHourAgo = DateTime.UtcNow.AddHours(-1); // delete any booking without paid after 12h
        
        var staleBookings = await _unitOfWork.Context.Set<Booking>()
            .Include(b => b.TourSchedule) 
            .Where(b => b.Status == BookingStatus.Pending && b.BookingDate < oneHourAgo)
            .ToListAsync();

        if (!staleBookings.Any())
        {
            _logger.LogInformation("No booking in the database");
            return; 
        }
        
        foreach (var booking in staleBookings)
        {
            booking.Status = BookingStatus.Cancelled;

            if (booking.TourSchedule != null)
            {
                booking.TourSchedule.AvailableSeats += booking.TicketCount;
            }
        }
        
        _unitOfWork.Context.UpdateRange(staleBookings);
        await _unitOfWork.CompleteAsync();
        
        _logger.LogInformation($"[Job] Cleaned up {staleBookings.Count} unpaid bookings.");
    }

    public async Task DeleteOldFilesAsync()
    {
        var thresholdDate = DateTime.UtcNow.AddDays(-7);

        
        var oldFiles = await _unitOfWork.Context.Set<FileRecord>()
            .IgnoreQueryFilters()
            .Where(f => f.IsDeleted && f.DeletedAt < thresholdDate)
            .ToListAsync();

        if (!oldFiles.Any())
        {
            _logger.LogInformation("No Files found");
            return; 
        }
        
        var oldFileIds = oldFiles.Select(f => f.Id).ToList();
        
        var relatedMedia = await _unitOfWork.Context.Set<TourMedia>()
            .Where(tm => oldFileIds.Contains(tm.FileId)).ToListAsync();

        if (relatedMedia.Any())
        {
            _unitOfWork.Context.Set<TourMedia>().RemoveRange(relatedMedia);
        }
        
        foreach (var file in oldFiles)
        {
            try
            {
                // Convert relative path to absolute system path
                
                var relativePath = file.FilePath.TrimStart('/', '\\');
                var fullPath = Path.Combine(_env.WebRootPath, relativePath);

                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                    _logger.LogInformation($"[Job] Deleted physical file: {fullPath}");
                }
            }
            catch (Exception ex)
            {
               _logger.LogError(ex,$"[Job] Error deleting file {file.FilePath}: {ex.Message}");
            }
        }

        //  Hard Delete from Database
        _unitOfWork.Context.Set<FileRecord>().RemoveRange(oldFiles);
        await _unitOfWork.CompleteAsync();

       _logger.LogInformation($"[Job] Removed {oldFiles.Count} old file records from DB.");
    }

    public async Task SendReviewRemindersAsync()
    {
        // between 24H and 48H
        var yesterday = DateTime.UtcNow.AddDays(-1);
        var startWindow = yesterday.AddHours(-12); 
        var endWindow = yesterday.AddHours(12);

        var bookingsToRemind = await _unitOfWork.Context.Set<Booking>()
            .Include(b => b.Tour)
            .Include(b => b.TourSchedule)
            .Include(b => b.Tourist).ThenInclude(t => t.User)
            .Where(b =>
                (b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.Completed) &&
                b.TourSchedule.StartTime.AddMinutes(b.Tour.DurationMinutes) >= startWindow &&
                b.TourSchedule.StartTime.AddMinutes(b.Tour.DurationMinutes) <= endWindow &&
                !_unitOfWork.Context.Set<Review>().Any(r => r.BookingId == b.Id))
            .ToListAsync();
        
        if (!bookingsToRemind.Any())
        {
            _logger.LogInformation("No bookings found");
            return;
        }

        foreach (var booking in bookingsToRemind)
        {
            if (booking.Tourist?.User == null) continue;
            var subject = $"How was your trip to {booking.Tour.Title}? ⭐";
        
            // ***Frontend Review Page
            var reviewLink = $"https://Tourist.com/bookings/{booking.Id}/write-review";

            var body = $@"
            <div style='font-family: Arial, sans-serif; padding: 20px;'>
                <h2>Welcome Back, {booking.Tourist.FullName}!</h2>
                <p>We hope you enjoyed your tour to <strong>{booking.Tour.Title}</strong>.</p>
                <p>Your feedback helps other travelers and helps our guides improve.</p>
                <br>
                <a href='{reviewLink}' style='background-color: #f1c40f; color: black; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>
                    ⭐⭐⭐⭐⭐ Rate Your Trip
                </a>
                <br><br>
                <p>It only takes 30 seconds!</p>
            </div>";
            
            try
            {
                await _emailService.SendEmailAsync(booking.Tourist.User.Email, subject, body);
            }
            catch (Exception ex)
            {
               _logger.LogError(ex,$"[Job] Failed to email booking {booking.Id}: {ex.Message}");
            }
        }

       _logger.LogInformation($"[Job] Sent {bookingsToRemind.Count} review reminders.");
            
        }

    public async Task CancelExpiredPaymentsAsync()
    {
        var threshold = DateTime.UtcNow.AddHours(-24);

        var expiredPayments = await _unitOfWork.Context.Set<Payment>()
            .Include(p => p.Booking)
            .Where(p => p.Status == PaymentStatus.Pending && p.CreatedAt < threshold)
            .ToListAsync();

        if (!expiredPayments.Any()) return;

        foreach (var payment in expiredPayments)
        {
            payment.Status = PaymentStatus.Cancelled;
            payment.FailureMessage = "System auto-cancelled: Session expired (24h+).";
            
            if (payment.Booking != null && payment.Booking.Status == BookingStatus.Pending)
            {
                payment.Booking.Status = BookingStatus.Cancelled;
            }
        }
        _unitOfWork.Context.UpdateRange(expiredPayments);
        await _unitOfWork.CompleteAsync();

        _logger.LogInformation($"[Job] Soft-deleted {expiredPayments.Count} expired payments.");
    }
}