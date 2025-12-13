using Microsoft.AspNetCore.Mvc;
using TouristsCore.Services;

[Route("api/[controller]")]
[ApiController]
public class TestEmailController : ControllerBase
{
    private readonly IEmailService _emailService;

    public TestEmailController(IEmailService emailService)
    {
        _emailService = emailService;
    }

    [HttpGet("send")]
    public async Task<IActionResult> SendTest()
    {
        try
        {
            string recipient = "omarradwan10a@gmail.com"; 
            string subject = "TourApp Test";
            string body = "<h1>It Works!</h1><p>This email was sent from your C# Code.</p>";

            await _emailService.SendEmailAsync(recipient, subject, body);

            return Ok(new { Message = "Email sent! Check your inbox." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }
}