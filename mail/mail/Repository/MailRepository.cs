using mail.Model;
using Microsoft.Extensions.Options;

namespace mail.Repository;

public class MailRepository
{
    private readonly List<Model.Mail> _mails = new();
    
    public MailRepository(IOptions<DatabaseSetting> databaseSettings)
    {
        if(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production")
        {
            var secret = Environment.GetEnvironmentVariable("MONGO_PASSWORD");
        }
    }

    public IEnumerable<Model.Mail> GEnumerable()
    {
        return _mails;
    }

    public Model.Mail? Get(Guid id)
    {
        return _mails.FirstOrDefault(o => o.Id == id);
    }

    public void Create(Model.Mail mail)
    {
        _mails.Add(mail);
    }

    public void Update(Guid id, Model.Mail newMail)
    {
        var target = _mails.FirstOrDefault(o => o.Id == id);
        if (target == null)
        {
            // TODO: error handling
        }
    }
}