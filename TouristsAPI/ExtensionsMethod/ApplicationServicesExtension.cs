using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TouristsAPI.ErrorResponses;
using TouristsAPI.Helpers;
using TouristsCore;
using TouristsCore.Entities;
using TouristsCore.Repositories;
using TouristsCore.Services;
using TouristsRepository;
using TouristsService;

namespace TouristsAPI.ExtensionsMethod;

public static class ApplicationServicesExtension
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<TouristsContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });

        services.AddIdentity<User, IdentityRole<Guid>>()
            .AddEntityFrameworkStores<TouristsContext>()
            .AddDefaultTokenProviders(); // to generate token for forgetPassword

        // Repositotries :
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();


        // Services :
        services.AddAutoMapper(typeof(MappingProfile));
        services.AddScoped<IAuthService,AuthService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IFileService,FileService>();
        services.AddScoped<IProfileService,ProfileService>();
        services.AddScoped<ITourService,TourService>();
        services.AddScoped<ITourScheduleService,TourScheduleService>();
        services.AddScoped<IBookingService,BookingService>();
        services.AddScoped<IReviewService,ReviewService>();
        services.AddScoped<IAdminService,AdminService>();
        services.AddScoped<IEmailService,EmailService>();
        
        
        #region Validation Error 
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = (context) =>
            {
                var errors = context.ModelState.Where(P => P.Value.Errors.Count() > 0)
                    .SelectMany(P => P.Value.Errors)
                    .Select(E => E.ErrorMessage)
                    .ToList();
                var ValidationErrorReposonse = new ApiValidationResponse()
                {
                    Errors = errors
                };
                return new BadRequestObjectResult(ValidationErrorReposonse);
        
            };
        }); // For Validation Error 
        #endregion

        return services;
    }
}
