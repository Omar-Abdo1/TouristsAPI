using Microsoft.OpenApi.Models;

namespace TouristsAPI.ExtensionsMethod;

public static class SwaggerExtension
{
    public static IServiceCollection AddSwaggerAdvanced(this IServiceCollection Services,string title = "WEB API",
        string description="API")
    {
        Services.AddEndpointsApiExplorer();
        Services.AddSwaggerGen(swagger =>
        {
            swagger.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = title,
                Description = description,
                Contact = new OpenApiContact
                {
                    Name = "Omar Abdo",
                    Email = "OmarRadwan10a@gmail.com"
                }
            });
            swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\"",
            });
            swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] {}
                }
            });
        });
        return Services;
    }
}