namespace mail.Model;

public class Mail
{
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
    
    public Mail(Guid id, MailStatus status)
    {
        Id = id;
        Status = status;
    }
    
    public Guid Id { get; set; }
    public MailStatus Status { get; set; }
}