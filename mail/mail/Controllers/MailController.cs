using mail.Model;
using mail.Repository;
using mail.Service;
using Microsoft.AspNetCore.Mvc;

namespace mail.Controllers;

[ApiController]
[Route("api/mail/")]
public class MailController : ControllerBase
{
    private readonly ILogger<MailController> _logger;
    private readonly MailRepository _mailRepositoryRepository;
    private readonly Pubsub _pubsub;

    public MailController(ILogger<MailController> logger, MailRepository mailRepositoryRepository, Pubsub pubsub)
    {
        _logger = logger;
        _mailRepositoryRepository = mailRepositoryRepository;
        _pubsub = pubsub;
    }

    [HttpGet("get/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MailListResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Response))]
    public ActionResult<MailListResponse> GetUserMails(string userId)
    {
        try
        {
            var mailList = _mailRepositoryRepository.GetMailData(userId);
            var response = new MailListResponse(mailList);
            return Ok(response);
        }
        catch (Exception e)
        {
            _logger.LogError("fetch mail error: {error}", e.Message);
            return BadRequest(new Response(e.Message));
        }
    }

    [HttpPost("create")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MailResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Response))]
    public async Task<ActionResult<MailResponse>> CreateMail(MailCreateRequest request)
    {
        var mail = new Mail();
        try
        {
            _mailRepositoryRepository.Create(mail, request.to);
        }
        catch (Exception e)
        {
            _logger.LogError("Create mail error: {error}", e.Message);
            return BadRequest(new Response(e.Message));
        }

        try
        {
            var msgId = await _pubsub.PublishMessageWithRetrySettingsAsync(mail.Id.ToString());
        }
        catch (Exception e)
        {
            _logger.LogError("Create event error: {error}", e.Message);
            _mailRepositoryRepository.UpdateMailStatus(mail.Id, MailStatus.FAILED);
            return BadRequest(new Response(e.Message));
        }

        try
        {
            _mailRepositoryRepository.UpdateMailStatus(mail.Id, MailStatus.SENDING);
        }
        catch (Exception e)
        {
            _logger.LogError("mail state error: {error}", e.Message);
            return BadRequest(new Response(e.Message));
        }

        _logger.LogInformation("Create mail: {mail}", mail.Id);
        return Ok(new MailResponse(mail));
    }

    [HttpPost("stop/{mailId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Response))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(Response))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Response))]
    public ActionResult<MailResponse> StopMail(string mailId)
    {
        try
        {
            var mail = _mailRepositoryRepository.GetMailData(Guid.Parse(mailId));
            if (mail == null)
            {
                _logger.LogWarning($"mail not found: {mailId}");
                return NotFound(new Response("mail not found"));
            }

            if (mail.Status != MailStatus.UNSEND && mail.Status != MailStatus.SENDING)
            {
                _logger.LogWarning($"mail in end state: {mailId}");
                return BadRequest(new Response("mail in end state"));
            }

            var newMail = _mailRepositoryRepository.StopMailSend(mail.Id, mail.Status);
            _logger.LogInformation("Stop mail: {mail}", mail.Id);
            return Ok(new MailResponse(newMail));
        }
        catch (Exception e)
        {
            _logger.LogError("Stop mail error: {error}", e.Message);
            return BadRequest(new Response(e.Message));
        }
    }
}