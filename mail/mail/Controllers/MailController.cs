using mail.Model;
using Microsoft.AspNetCore.Mvc;

namespace mail.Controllers;

[ApiController]
[Route("api/mail/")]
public class MailController : ControllerBase
{
    private readonly ILogger<MailController> _logger;
    private readonly Repository.MailRepository _mailRepositoryRepository;

    public MailController(ILogger<MailController> logger, Repository.MailRepository mailRepositoryRepository)
    {
        _logger = logger;
        _mailRepositoryRepository = mailRepositoryRepository;
    }
    
    [HttpGet("get/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MailListResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Response))]
    public ActionResult<MailListResponse> GetUserMails(string userId)
    {
        try
        {
            var mail = _mailRepositoryRepository.Get(Guid.Parse(userId));
            if (mail == null)
            {
                return NotFound(new Response("mail not found"));
            }
            var response = new MailResponse(mail);
            return Ok(response);
        }
        catch (Exception e)
        {
            _logger.LogError("fetch mail error: {error}", e.Message);
            return NotFound(new Response(e.Message));
        }
    }
    
    [HttpPost("create")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MailResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Response))]
    public ActionResult<MailResponse> CreateMail(MailCreateRequest request)
    {
        var mail = new Mail();
        _logger.LogInformation("Create mail: {mail}", mail.Id);
        return Ok(new MailResponse(mail));
    }
    
    [HttpPost("stop/{mailId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MailResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Response))]
    public ActionResult<MailResponse> StopMail(string mailId)
    {
        try {
            var mail = new Mail(Guid.Parse(mailId));
            _logger.LogInformation("Stop mail: {mail}", mail.Id);
            return Ok(new MailResponse(mail));
        } catch (Exception e) {
            _logger.LogError("Stop mail error: {error}", e.Message);
            return NotFound(new Response(e.Message));
        }
    }
}