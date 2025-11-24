
using Bogus;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TouristsCore.Entities;
using TouristsRepository;

public class DbSeeder
{
    private const string Password = "Pa$$w0rd";
    private readonly TouristsContext _context;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;

    public DbSeeder(TouristsContext context, UserManager<User> userManager, RoleManager<IdentityRole<Guid>> roleManager)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task SeedAsync()
    {
        
        if (await _context.Users.AnyAsync()) return;

        await CreateRolesAsync();

        await CreateLanguagesAsync();

        await CreateUsersAndProfilesAsync();

        await CreateToursAsync();

        await _context.SaveChangesAsync();
    }

    private async Task CreateRolesAsync()
    {
        string[] roles = { "Admin", "Guide", "Tourist" };
        foreach (var role in roles)
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                await _roleManager.CreateAsync(new IdentityRole<Guid>(role));
            }
        }
    }

    private async Task CreateLanguagesAsync()
    {
        var languages = new List<Language>
        {
            new Language { Name = "English"},
            new Language { Name = "Arabic"  },
            new Language { Name = "French" },
            new Language { Name = "Spanish"  },
            new Language { Name = "German"}
        };

        await _context.Languages.AddRangeAsync(languages);
        await _context.SaveChangesAsync();
    }

    private async Task CreateUsersAndProfilesAsync()
    {
        var guideFaker = new Faker<User>()
            .RuleFor(u => u.UserName, f => f.Internet.UserName())
            .RuleFor(u => u.Email, f => f.Internet.Email())
            .RuleFor(u => u.PhoneNumber, f => f.Phone.PhoneNumber())
            .RuleFor(u => u.IsActive, true);

        var guides = guideFaker.Generate(20); // Generate 20 Guides

        foreach (var user in guides)
        {
            await _userManager.CreateAsync(user, Password);
            await _userManager.AddToRoleAsync(user, "Guide");

            var guideProfile = new GuideProfile
            {
                UserId = user.Id,
                Bio = new Faker().Lorem.Paragraph(),
                ExperienceYears = new Faker().Random.Int(1, 20),
                RatePerHour = new Faker().Random.Decimal(10, 100),
                IsVerified = true
            };
            await _context.GuideProfiles.AddAsync(guideProfile);
            
            var randomLangId = _context.Languages.Local.OrderBy(r => Guid.NewGuid()).First().Id;
            await _context.GuideLanguages.AddAsync(new GuideLanguage 
            { 
                GuideProfileId = guideProfile.Id, 
                LanguageId = randomLangId 
            });
        }

        var tourists = guideFaker.Generate(40); // Generate 400 Tourists (using same faker rules)

        foreach (var user in tourists)
        {
            await _userManager.CreateAsync(user, Password);
            await _userManager.AddToRoleAsync(user, "Tourist");

            var touristProfile = new TouristProfile
            {
                UserId = user.Id,
                FullName = new Faker().Name.FullName(), // Real sounding name
                Country = new Faker().Address.Country(),
                Phone = user.PhoneNumber
            };
            await _context.TouristProfiles.AddAsync(touristProfile);
        }
        await _context.SaveChangesAsync();
    }

    private async Task CreateToursAsync()
    {
        var guideIds = _context.GuideProfiles.Select(g => g.Id).ToList();

        var tourFaker = new Faker<Tour>()
            .RuleFor(t => t.Title, f => f.Commerce.ProductName() + " Tour")
            .RuleFor(t => t.Description, f => f.Lorem.Paragraphs(2))
            .RuleFor(t => t.Price, f => decimal.Parse(f.Commerce.Price(50, 500)))
            .RuleFor(t => t.DurationMinutes, f => f.PickRandom(60, 120, 180, 240))
            .RuleFor(t => t.City, f => f.Address.City()) 
            .RuleFor(t => t.Country, f => f.Address.Country())
            .RuleFor(t => t.IsPublished, true);

        var tours = tourFaker.Generate(30); // Create 30 fake tours

        foreach (var tour in tours)
        {
            tour.GuideProfileId = new Faker().PickRandom(guideIds);
            await _context.Tours.AddAsync(tour);
        }
    }
}