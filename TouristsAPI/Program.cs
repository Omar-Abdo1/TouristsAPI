using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using TouristsAPI.ExtensionsMethod;
using TouristsAPI.MiddleWares;
using TouristsCore.Entities;
using TouristsRepository;
using TouristsService;

namespace TouristsAPI;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddControllers().AddJsonOptions(options =>
        {
            // Add the converter to use string names for all enums
            options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
        });
        
        builder.Services.AddControllers();
        builder.Services.AddSwaggerGen();
        builder.Services.AddOpenApi();

        builder.Services.AddApplicationServices(
            builder.Configuration.GetConnectionString("DefaultConnection"));
        
        builder.Services.AddJWTServices(builder.Configuration["JWT:SecretKey"],
            builder.Configuration["JWT:IssuerIP"]);
        
        
        builder.Services.AddSwaggerAdvanced("TouristsAPI");
        
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("My Policy", options =>
            {
                options.AllowAnyHeader();
                options.AllowAnyMethod();
                options.WithOrigins(builder.Configuration["FrontBaseUrl"]); // URL for The FrontEnd
            });
        });
        
        builder.Services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(builder.
                Configuration.GetConnectionString("DefaultConnection")));
        /*
         *"This configures Hangfire to store jobs in SQL Server so they persist after restarts.
         * The Serializer settings ensure that my C# objects are converted to JSON and back correctly,
         * even if I update my application version."
         */

        builder.Services.AddHangfireServer();  
        
        builder.Services.Configure<EmailSettings>(
            builder.Configuration.GetSection("EmailSettings")
        ); // go get the values from app settings.json


        builder.Host.UseSerilog((context, configuration) =>
            configuration.ReadFrom.Configuration(context.Configuration));
        
        
        
        var app = builder.Build();
        await app.UpdateDatabaseAsync();

        app.UseMiddleware<ExceptionMiddleWare>(); // custom MiddleWare

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.MapOpenApi();
        }
        
        app.UseCors("My  Policy");
        app.UseStaticFiles();
        
        app.UseStatusCodePagesWithReExecute("/error/{0}");

        app.UseSerilogRequestLogging();
        
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        
        app.UseHangfireDashboard("/hangfire");
        
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
        
        await app.RunAsync();
    }
}