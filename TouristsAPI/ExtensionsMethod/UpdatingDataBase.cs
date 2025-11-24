using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TouristsCore.Entities;
using TouristsRepository;

namespace TouristsAPI.ExtensionsMethod;

public static class UpdatingDataBase
{
    public static async Task UpdateDatabaseAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var Services = scope.ServiceProvider;
        var LoggerFactory = Services.GetRequiredService<ILoggerFactory>();
        try
        {           
            var dbContext = Services.GetRequiredService<TouristsContext>();
            var userManger = Services.GetRequiredService<UserManager<User>>();
            var roleManger = Services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
            await dbContext.Database.MigrateAsync();
            
            // Seed data if necessary
            var seeder = new DbSeeder(dbContext, userManger, roleManger);
            await seeder.SeedAsync();
        }
        catch (Exception ex)
        {
            var Logger = LoggerFactory.CreateLogger<Program>();
            Logger.LogError(ex, "An error occurred while migrating the database.");
        }
    }
}