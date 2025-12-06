using Microsoft.AspNetCore.Mvc;
using TouristsAPI.ErrorResponses;

namespace TouristsAPI.Controllers;

[ApiController]
[Route("error/{code}")]
[ApiExplorerSettings(IgnoreApi = true)]
public class ErrorController : ControllerBase
{
    public ActionResult Error(int code)
    {
        return NotFound(new ApiErrorResponse(code,"The requested resource was not found."));
    }
}