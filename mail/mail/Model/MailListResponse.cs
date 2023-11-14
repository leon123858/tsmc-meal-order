namespace mail.Model;

public class MailListResponse : Response
{
    public List<UserMail> data { get; set; }
    
    public MailListResponse(List<Mail> data) : base(data)
    {
        this.data = data.Select(mail => new UserMail(mail)).ToList();
    }
    
    public MailListResponse(string message) : base(message)
    { }
}