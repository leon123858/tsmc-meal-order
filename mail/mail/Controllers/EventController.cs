namespace mail.Controllers;

using mail.Model;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/mail/")]
public class EventController : ControllerBase
{
    private readonly ILogger<EventController> _logger;

    public EventController(ILogger<EventController> logger)
    {
        _logger = logger;
    }
    
    [HttpPost("send/{mailId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    public ActionResult<string> SendMail(string mailId)
    {
        // gcp send mail by sendGrid
        return Ok("success");
    }
}