using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TouristsCore.Entities;
using TouristsRepository;

namespace TouristsAPI;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddControllers();
        builder.Services.AddSwaggerGen();
        builder.Services.AddOpenApi();

        builder.Services.AddDbContext<TouristsContext>(options =>
        {
          options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
        });

        builder.Services.AddIdentity<User, IdentityRole<Guid>>()
            .AddEntityFrameworkStores<TouristsContext>();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.MapOpenApi();
        }
        
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}