using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using TouristsAPI.ErrorResponses;
using TouristsCore.Services;

namespace TouristsAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class FileController : ControllerBase
{
    private readonly IFileService _fileService;

    public FileController(IFileService  fileService)
    {
        _fileService = fileService;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload([FromBody] IFormFile file, [FromQuery]string folderName = "common")
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        try
        {
            var fileRecord = await _fileService.SaveFileAsync(file, folderName,Guid.Parse(userId));
            return Ok(new
            {
             Message = "Uploaded successfully",
             FileId =  fileRecord.Id,
             Url = fileRecord.FilePath
            });
        }
        catch (ArgumentException ex)
        {
                return BadRequest(new ApiErrorResponse(400, ex.Message));
        } 
    }
    
    
}