using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TouristsAPI.ExtensionsMethod;
using TouristsAPI.MiddleWares;
using TouristsCore.Entities;
using TouristsRepository;

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
        
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        await app.RunAsync();
    }
}