namespace mail.Model;

public class Mail
{
    public Guid Id { get; set; }
    public MailStatus Status { get; set; }
    
    public Mail()
    {
        Id = Guid.NewGuid();
        Status = MailStatus.UNSEND;
    }
    
    public Mail(Guid id)
    {
        Id = id;
        Status = MailStatus.UNSEND;
    }
}