using System.Text.Json;
using TouristsAPI.ErrorResponses;

namespace TouristsAPI.MiddleWares;

public class ExceptionMiddleWare
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleWare> _logger;
    private readonly IHostEnvironment _environment;

    public ExceptionMiddleWare(RequestDelegate next, ILogger<ExceptionMiddleWare> logger, IHostEnvironment environment, IConfiguration configuration)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next.Invoke(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = 500; // Internal Server Error
            var response = _environment.IsDevelopment()
                ? new ApiExceptionResponse(500, ex.Message, ex.StackTrace?.ToString())
                : new ApiExceptionResponse(500);
                
            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var json = JsonSerializer.Serialize(response, options);
            await context.Response.WriteAsync(json);
        }
    }
}
