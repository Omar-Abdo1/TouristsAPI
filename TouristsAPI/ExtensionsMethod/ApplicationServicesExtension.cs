using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TouristsCore.Entities;
using TouristsRepository;

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
            .AddEntityFrameworkStores<TouristsContext>();

        // Repositotries :
       // services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
       // services.AddScoped<IUnitOfWork, UnitOfWork>();



        // Services :
        

        // #region Validation Error 
        // services.Configure<ApiBehaviorOptions>(options =>
        // {
        //     options.InvalidModelStateResponseFactory = (context) =>
        //     {
        //         var errors = context.ModelState.Where(P => P.Value.Errors.Count() > 0)
        //             .SelectMany(P => P.Value.Errors)
        //             .Select(E => E.ErrorMessage)
        //             .ToList();
        //         var ValidationErrorReposonse = new ApiValidationResponse()
        //         {
        //             Errors = errors
        //         };
        //         return new BadRequestObjectResult(ValidationErrorReposonse);
        //
        //     };
        // }); // For Validation Error 
        // #endregion

        return services;
    }
}
