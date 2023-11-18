namespace mail.Model;

public class MailEvent
{
    public MailEvent(string to, string mailId, string subject, string content)
    {
        To = to;
        MailId = mailId;
        Subject = subject;
        Content = content;
    }

    public string To { get; set; }
    public string Subject { get; set; }
    public string MailId { get; set; }
    public string Content { get; set; }
}