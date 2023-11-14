namespace mail.Model;

public class UserMail: Mail
{
    public string Status { get; set; }
    
    public UserMail(Mail mail)
    {
        Id = mail.Id;
        Status = mail.Status switch
        {
            MailStatus.UNSEND => "待寄出",
            MailStatus.SENDING => "寄送中",
            MailStatus.FAILED => "寄送失敗",
            MailStatus.STOPPED => "已停止",
            MailStatus.SENT => "已寄出",
            _ => "錯誤狀態"
        };
    }
}