namespace mail.Model;

public class MailCreateRequest
{
    public string to { get; set; }
    public string subject { get; set; }
    public string body { get; set; }
}