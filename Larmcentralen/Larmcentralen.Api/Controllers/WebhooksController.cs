using Larmcentralen.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Larmcentralen.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WebhooksController(SmsAlarmService smsService) : ControllerBase
{
    [HttpPost("sms")]
    public async Task<IActionResult> ReceiveSms([FromForm] string from, [FromForm] string message)
    {
        await smsService.ProcessSmsAsync(from, message);
        return Ok();
    }
}