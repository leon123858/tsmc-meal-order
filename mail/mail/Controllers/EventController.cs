using mail.Model;
using mail.Repository;
using mail.Service;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace mail.Controllers;

[ApiController]
[Route("api/mail/")]
public class EventController : ControllerBase
{
    private readonly ILogger<EventController> _logger;
    private readonly MailRepository _mailRepositoryRepository;
    private readonly MailService _mailService;

    public EventController(ILogger<EventController> logger, MailService mailService,
        MailRepository mailRepositoryRepository)
    {
        _logger = logger;
        _mailService = mailService;
        _mailRepositoryRepository = mailRepositoryRepository;
    }

    [HttpPost("event/{type}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    public ActionResult<string> SendMail(string type, PubSubMessage message)
    {
        string mailData;
        try
        {
            mailData = Pubsub.ReceiveMessageData(message);
        }
        catch (Exception e)
        {
            _logger.LogError("Fetch mail data error: {error}", e.Message);
            return BadRequest("Fetch mail data error");
        }

        switch (type)
        {
            case "send-mail-event":
                try
                {
                    var mailEvent = JsonConvert.DeserializeObject<MailEvent>(mailData);
                    _mailService.SendEmail(mailEvent);
                    _mailRepositoryRepository.UpdateMailStatus(Guid.Parse(mailEvent.MailId), MailStatus.SENT);
                }
                catch (Exception e)
                {
                    _logger.LogError("Send mail error: {error}", e.Message);
                    return BadRequest("Send mail error");
                }

                break;
            case "fail-mail-event":
                try
                {
                    var mailEvent = JsonConvert.DeserializeObject<MailEvent>(mailData);
                    _mailRepositoryRepository.UpdateMailStatus(Guid.Parse(mailEvent.MailId), MailStatus.FAILED);
                }
                catch (Exception e)
                {
                    _logger.LogError("Fail mail error: {error}", e.Message);
                    return BadRequest("Fail mail error");
                }

                break;
            default:
                // _logger.LogError("data: {mailData}", mailData);
                _logger.LogError("Invalid event type: {type}", type);
                return Ok("wrong event type");
        }

        return Ok("success");
    }
}