using order.Model;

namespace order.Service;

public interface IMailService
{
    void SendOrderCreatedMail(Order order);
    void SendOrderConfirmedMail(Order order);
    void SendOrderDeletedMail(Order order);
    void SendOrderNotifyMail(Order order);
}