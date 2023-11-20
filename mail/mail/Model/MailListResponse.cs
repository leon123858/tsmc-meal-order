namespace mail.Model;

public class MailListResponse : Response
{
    public MailListResponse(List<Mail> data) : base(data)
    {
        this.data = data.Select(mail => new UserMail(mail)).ToList();
    }

    public MailListResponse(string message) : base(message)
    {
    }

    public List<UserMail> data { get; set; }
}