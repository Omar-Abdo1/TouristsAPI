using Hangfire;
using TouristsService;

namespace TouristsAPI.ExtensionsMethod;

public static class AddingBackGroundJobs
{
    public static WebApplication AddingrecurringJobs(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();

            recurringJobManager.AddOrUpdate<IJobService>(
                "cancel-unpaid-bookings", 
                job => job.AutoCancelUnpaidBookings(), 
                Cron.Hourly
            );
            
            recurringJobManager.AddOrUpdate<IJobService>(
                "cleanup-old-files",job=>job.
                    DeleteOldFilesAsync(),
                Cron.Daily);
            
            recurringJobManager.AddOrUpdate<IJobService>(
                "send-review-reminders",job=>job.
                    SendReviewRemindersAsync(),
                Cron.Daily(10));
        }

        return app;
    }
}