namespace mail.Model;

public class MailResponse : Response
{
    public UserMail data { get; set; }
    
    public MailResponse(Mail data) : base(data)
    {
        this.data = new UserMail(data);
    }
    
    public MailResponse(string message) : base(message)
    { }
}