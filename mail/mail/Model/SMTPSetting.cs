namespace mail.Model;

public class SMTPSetting
{
    public string Host { get; set; } = null!;
    public string Sender { get; set; } = null!;
    public string Password { get; set; } = null!;
}