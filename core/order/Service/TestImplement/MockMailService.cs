using order.Model;

namespace order.Service.TestImplement;

public class MockMailService : IMailService
{
    private readonly ILogger<MockMailService> _logger;

    public MockMailService(ILogger<MockMailService> logger)
    {
        _logger = logger;
    }
    
    public void SendOrderCreatedMail(Order order)
    {
        LogMail(order);
    }

    private void LogMail(Order order)
    {
        _logger.LogInformation("SendOrderCreatedMail: {CustomerName} {OrderOrderDate} {OrderMealType}", order.Customer.Name, order.OrderDate, order.MealType);
    }

    public void SendOrderConfirmedMail(Order order)
    {
        LogMail(order);
    }

    public void SendOrderDeletedMail(Order order)
    {
        LogMail(order);
    }

    public void SendOrderNotifyMail(Order order)
    {
        LogMail(order);
    }
}