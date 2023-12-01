using System.Text;
using core.Model;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using order.Config;
using order.DTO.Web;
using order.Model;
using order.Repository.WebImplement;

namespace order.Service;

public class MailService
{
    private readonly WebUtils _webUtils;
    private readonly ILogger<MailService> _logger;

    public MailService(IOptions<WebConfig> config, ILogger<MailService> logger)
    {
        _webUtils = new WebUtils(config.Value.MailUrl);
        _logger = logger;
    }

    public void SendOrderCreatedMail(Order order)
    {
        SendMail(order.Customer.Email, "餐點訂單已建立",
            $"以下訂單已建立\r\n日期：{order.OrderDate:yyyy/MM/dd}\r\n餐點：{GetFoodItemsString(order.FoodItems)}");
    }

    public void SendOrderConfirmedMail(Order order)
    {
        SendMail(order.Customer.Email, "餐點訂單已確認",
            $"以下訂單已確認\r\n日期：{order.OrderDate:yyyy/MM/dd}\r\n餐點：{GetFoodItemsString(order.FoodItems)}");
    }

    public void SendOrderDeletedMail(Order order)
    {
        SendMail(order.Customer.Email, "餐點訂單已刪除",
            $"以下訂單已取消\r\n日期：{order.OrderDate:yyyy/MM/dd}\r\n餐點：{GetFoodItemsString(order.FoodItems)}");
    }

    private string GetFoodItemsString(List<FoodItem> orderFoodItems)
    {
        var stringBuilder = new StringBuilder();
        foreach (var foodItem in orderFoodItems) stringBuilder.Append($"{foodItem.Name} x {foodItem.Count} \r\n");

        return stringBuilder.ToString();
    }

    private async Task SendMail(string to, string subject, string body)
    {
        const string endPoint = "/create";

        try
        {
            var mail = new CreateMailWebDTO() { to = to, subject = subject, body = body };
            var mailJson = JsonConvert.SerializeObject(mail);

            var apiResponse = await _webUtils.PostAsync<MailResponseWebDTO>(endPoint, mailJson);

            if (apiResponse is { Result: false })
                _logger.LogError("Mail sent to {To} with subject {Subject} failed: {Error}", to, subject,
                    apiResponse.Message);
        }
        catch (Exception e)
        {
            _logger.LogError("Mail sent to {To} with subject {Subject} failed: {Error}", to, subject, e.Message);
        }
    }
}