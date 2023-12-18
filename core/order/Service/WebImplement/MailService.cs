using System.Text;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using order.Config;
using order.DTO.Web;
using order.Model;
using order.Repository.WebImplement;

namespace order.Service.WebImplement;

public class MailService : IMailService
{
    private readonly ILogger<MailService> _logger;
    private readonly WebUtils _webUtils;

    public MailService(IOptions<WebConfig> config, ILogger<MailService> logger)
    {
        _webUtils = new WebUtils(config.Value.MailUrl);
        _logger = logger;
    }

    public void SendOrderCreatedMail(Order order)
    {
        SendMail(order.Customer.Email, "餐點訂單已建立",
            $"以下訂單已建立\r\n日期：{order.OrderDate:yyyy/MM/dd}\r\n\r\n餐點：{GetFoodItemsString(order.FoodItems)}");
        SendMail(order.Restaurant.Email, "餐點訂單已建立",
            $"以下訂單已建立\r\n日期：{order.OrderDate:yyyy/MM/dd}\r\n\r\n餐點：{GetFoodItemsString(order.FoodItems)}");
    }

    public void SendOrderConfirmedMail(Order order)
    {
        SendMail(order.Customer.Email, "餐點訂單已確認",
            $"以下訂單已確認\r\n日期：{order.OrderDate:yyyy/MM/dd}\r\n\r\n餐點：{GetFoodItemsString(order.FoodItems)}");
    }

    public void SendOrderDeletedMail(Order order)
    {
        SendMail(order.Customer.Email, "餐點訂單已刪除",
            $"以下訂單已取消\r\n日期：{order.OrderDate:yyyy/MM/dd}\r\n\r\n餐點：{GetFoodItemsString(order.FoodItems)}");
    }

    public void SendOrderNotifyMail(Order order)
    {
        SendMail(order.Customer.Email, "餐點訂單通知",
            $"請記得於{GetMealTypeString(order.MealType)}時間前來取餐\r\n日期：{order.OrderDate:yyyy/MM/dd}\r\n\r\n餐點：{GetFoodItemsString(order.FoodItems)}");
    }

    private string GetFoodItemsString(List<OrderedFoodItem> orderFoodItems)
    {
        var stringBuilder = new StringBuilder();
        foreach (var foodItem in orderFoodItems)
            stringBuilder.Append($"{foodItem.Snapshot.Name} x {foodItem.Count} : {foodItem.Description} \r\n");

        return stringBuilder.ToString();
    }

    private string GetMealTypeString(MealType mealType)
    {
        return mealType switch
        {
            MealType.Breakfast => "早餐",
            MealType.Lunch => "午餐",
            MealType.Dinner => "晚餐",
            _ => throw new ArgumentOutOfRangeException(nameof(mealType), mealType, null)
        };
    }

    private async Task SendMail(string to, string subject, string body)
    {
        const string endPoint = "/create";

        try
        {
            var mail = new CreateMailWebDTO { to = to, subject = subject, body = body };
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