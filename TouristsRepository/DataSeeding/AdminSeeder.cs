using Microsoft.AspNetCore.Identity;
using TouristsCore.Entities;

namespace TouristsRepository.DataSeeding;

public static class AdminSeeder
{
    private const string AdminPassword = "Pa$$w0rd!123";
    public static async Task SeedUsersAsync(UserManager<User> userManager)
    {
        if (await userManager.FindByEmailAsync("admin@tourist.com") == null)
        {
            var user = new User
            {
                UserName = "Admin",
                Email = "admin@tourist.com",
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(user, AdminPassword);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "Admin");
            }
        }
    }
}