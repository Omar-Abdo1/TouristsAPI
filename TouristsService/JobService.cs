using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TouristsCore;
using TouristsCore.Entities;
using TouristsCore.Enums;
using TouristsCore.Enums.Payment;
using TouristsCore.Services;
using TouristsRepository;

namespace TouristsService;

public interface IJobService
{
    Task AutoCancelUnpaidBookings();
    Task DeleteOldFilesAsync();

    Task SendReviewRemindersAsync();
    
    Task CancelExpiredPaymentsAsync();

    Task AutoCompleteFinishedBookings();
}

public class JobService : IJobService
{
    private readonly TouristsContext _context;
    private readonly IWebHostEnvironment _env;
    private readonly IEmailService _emailService;
    private readonly ILogger<JobService> _logger;

    public JobService(TouristsContext context,IWebHostEnvironment env,IEmailService emailService,ILogger<JobService>  logger)
    {
        _context = context;
        _env = env;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task AutoCancelUnpaidBookings()
    {
        var timeoutThreshold = DateTime.UtcNow.AddHours(-1); 
        
        var staleBookingIds = await _context.Bookings
            .AsNoTracking() 
            .Where(b => b.Status == BookingStatus.Pending && b.BookingDate < timeoutThreshold)
            .Select(b => b.Id)
            .ToListAsync();

        if (!staleBookingIds.Any()) return;

        int successCount = 0;

        foreach (var id in staleBookingIds)
        {
            try
            {
                var booking = await _context.Bookings.FindAsync(id);
                
                if (booking == null || booking.Status != BookingStatus.Pending) 
                    continue; 

                booking.Status = BookingStatus.Cancelled;
                _context.Bookings.Update(booking);
                
                await _context.SaveChangesAsync();
                successCount++;
            }
            catch (DbUpdateConcurrencyException)
            {
                _logger.LogWarning($"[Job] Concurrency conflict for Booking {id}. User likely paid just now. Skipping.");
                _context.ChangeTracker.Clear();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Job] Error cancelling booking {Id}", id);
                _context.ChangeTracker.Clear();
            }
        }
        
        if(successCount > 0)
            _logger.LogInformation($"[Job] Cleaned up {successCount} unpaid bookings.");
    }


    public async Task AutoCompleteFinishedBookings()
    {
        var now = DateTime.UtcNow;

        var finishedBookingIds = await _context.Bookings
            .Include(b=>b.Tour).Include(b=>b.TourSchedule)
            .Where(b => b.Status == BookingStatus.Confirmed &&
                        b.TourSchedule.StartTime.AddMinutes(b.Tour.DurationMinutes) < now)
            .Select(b => b.Id)
            .ToListAsync();
        
        if(!finishedBookingIds.Any()) return;

        int successCount = 0;

        foreach (var id in finishedBookingIds)
        {
            try
            {
                var booking = await _context.Bookings
                    .Include(b => b.Tour) 
                    .Include(b => b.Tourist).ThenInclude(t => t.User) 
                    .FirstOrDefaultAsync(b => b.Id == id);
                
                if (booking == null || booking.Status != BookingStatus.Confirmed) 
                    continue; 

                booking.Status = BookingStatus.Completed;
                await _context.SaveChangesAsync();
                
                var subject = $"How was your trip to {booking.Tour.Title}? ⭐";
                var reviewLink = $"https://Tourist.com/bookings/{booking.Id}/write-review";

                var body = $@"<div style='font-family: Arial, sans-serif; padding: 20px;'>
                            <h2>Welcome Back, {booking.Tourist.FullName}!</h2>
                            <p>We hope you enjoyed your tour to <strong>{booking.Tour.Title}</strong>.</p>
                            <a href='{reviewLink}'>⭐⭐⭐⭐⭐ Rate Your Trip Now</a>
                          </div>";
                try
                {
                    if (booking.Tourist?.User?.Email != null)
                    {
                        await _emailService.SendEmailAsync(booking.Tourist.User.Email, subject, body);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"[Job] Booking {id} completed, but email failed.");
                }
                
                successCount++;
            }
            catch (DbUpdateConcurrencyException)
            {
                _logger.LogWarning($"[Job] Concurrency conflict for Booking {id}. Skipping.");
                _context.ChangeTracker.Clear();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Job] Error Completing booking {Id}", id);
                _context.ChangeTracker.Clear();
            }
        }
        
        if(successCount > 0)
            _logger.LogInformation($"[Job] Marked {successCount} bookings as Completed.");
    }

    public async Task SendReviewRemindersAsync()
    {
        var yesterday = DateTime.UtcNow.AddDays(-1);
        var startWindow = yesterday.AddHours(-12); 
        var endWindow = yesterday.AddHours(12);

        var bookingsToRemind = await _context.Bookings
            .AsNoTracking()
            .Include(b => b.Tour)
            .Include(b => b.TourSchedule)
            .Include(b => b.Tourist).ThenInclude(t => t.User)
            .Where(b =>
                (b.Status == BookingStatus.Completed) && 
                b.TourSchedule.StartTime.AddMinutes(b.Tour.DurationMinutes) >= startWindow &&
                b.TourSchedule.StartTime.AddMinutes(b.Tour.DurationMinutes) <= endWindow &&
                !_context.Set<Review>().Any(r => r.BookingId == b.Id))
            .ToListAsync();
        
        if (!bookingsToRemind.Any()) return;

        int sentCount = 0;

        foreach (var booking in bookingsToRemind)
        {
            if (booking.Tourist?.User == null || string.IsNullOrEmpty(booking.Tourist.User.Email)) 
                continue;
            
            var subject = $"Reminder: Rate your trip to {booking.Tour.Title} ✍️"; // Changed Subject
            var reviewLink = $"https://Tourist.com/bookings/{booking.Id}/write-review";

            var body = $@"<div style='font-family: Arial, sans-serif; padding: 20px;'>
                            <h2>Hi {booking.Tourist.FullName},</h2>
                            <p>It's been a day since your tour to <strong>{booking.Tour.Title}</strong>.</p>
                            <p>In case you forgot, we'd love to hear your thoughts!</p>
                            <a href='{reviewLink}'>⭐⭐⭐⭐⭐ Write a Review</a>
                          </div>";
            try
            {
                await _emailService.SendEmailAsync(booking.Tourist.User.Email, subject, body);
                sentCount++;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[Job] Failed to email reminder for booking {booking.Id}");
            }
        }

        if (sentCount > 0)
            _logger.LogInformation($"[Job] Sent {sentCount} review reminders.");
    }

    public async Task CancelExpiredPaymentsAsync()
    {
        var threshold = DateTime.UtcNow.AddHours(-24);

        var expiredPayments = await _context.Set<Payment>()
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
                _context.Entry(payment.Booking).State = EntityState.Modified;
            }
        }

        try 
        {
            _context.UpdateRange(expiredPayments);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"[Job] Soft-deleted {expiredPayments.Count} expired payments.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[Job] Failed to batch cancel expired payments.");
            _context.ChangeTracker.Clear();
        }
    }
    public async Task DeleteOldFilesAsync()
    {
        var thresholdDate = DateTime.UtcNow.AddDays(-7);

        var oldFiles = await _context.Set<FileRecord>()
            .IgnoreQueryFilters() 
            .Where(f => f.IsDeleted && f.DeletedAt < thresholdDate)
            .ToListAsync();

        await ProcessFileDeletion(oldFiles);
        
        var ghostThreshold = DateTime.UtcNow.AddHours(-24);

        // Find files that are OLD, NOT Deleted, and NOT linked to anything
        var ghostFiles = await _context.Set<FileRecord>()
            .Where(f => f.CreatedAt < ghostThreshold && !f.IsDeleted) 
            .Where(f => !_context.Set<TourMedia>().Any(tm => tm.FileId == f.Id))
            .Where(f => !_context.Set<Message>().Any(m => m.AttachmentFileId == f.Id)) // Not in Chat
             .Where(f => !_context.TouristProfiles.Any(u => u.AvatarFileId == f.Id)) 
            .Where(f => !_context.GuideProfiles.Any(g => g.AvatarFileId == f.Id)) 
            .ToListAsync();

        if (ghostFiles.Any())
        {
            _logger.LogInformation($"[Job] Found {ghostFiles.Count} abandoned ghost files.");
            await ProcessFileDeletion(ghostFiles);
        }
        
    }
    
    private async Task ProcessFileDeletion(List<FileRecord> filesToDelete)
    {
        if (!filesToDelete.Any()) return;
        
        var fileIds = filesToDelete.Select(f => f.Id).ToList();
        var relatedMedia = await _context.Set<TourMedia>()
            .Where(tm => fileIds.Contains(tm.FileId))
            .ToListAsync();

        if (relatedMedia.Any())
        {
            _context.Set<TourMedia>().RemoveRange(relatedMedia);
            await _context.SaveChangesAsync();
        }

        int count = 0;
        foreach (var file in filesToDelete)
        {
            try
            {
                if (!string.IsNullOrEmpty(file.FilePath))
                {
                    var relativePath = file.FilePath.TrimStart('/', '\\');
                    var fullPath = Path.Combine(_env.WebRootPath, relativePath);

                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                        _logger.LogInformation($"[Job] Deleted physical file: {fullPath}");
                    }
                }
                _context.Set<FileRecord>().Remove(file);
                await _context.SaveChangesAsync();
                ++count;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogWarning($"[Job] Skipped file {file.Id} - Still in use. Details: {ex.InnerException?.Message}");
                _context.Entry(file).State = EntityState.Detached;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[Job] Error deleting file {file.Id}");
                _context.ChangeTracker.Clear();
            }
        }
    
        if (count > 0)
            _logger.LogInformation($"[Job] Successfully scrubbed {count} files from database.");
    }
}
