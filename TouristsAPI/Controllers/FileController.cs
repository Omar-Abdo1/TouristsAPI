using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.RateLimiting;
using TouristsAPI.ErrorResponses;
using TouristsCore.Entities;
using TouristsCore.Services;

namespace TouristsAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
[EnableRateLimiting("Global")]
public class FileController : ControllerBase
{
    private readonly IFileService _fileService;

    public FileController(IFileService  fileService)
    {
        _fileService = fileService;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile file, [FromQuery]string folderName = "common")
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
    
    [Authorize]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteFile(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        bool isAdmin = User.IsInRole("Admin");
        try
        {
            await _fileService.DeleteFileAsync(id,Guid.Parse(userId),isAdmin);
            return Ok(new
            {
                Message = "File Moved To Trash Successfully"
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ApiErrorResponse(404, ex.Message));
        }
        catch (UnauthorizedAccessException ex)
        {
             return Unauthorized(new ApiErrorResponse(403, ex.Message));
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiErrorResponse(400, ex.Message));
        }
    }
    
    
}