using core.Model;
using Microsoft.Extensions.Options;
using order.Config;
using order.Exceptions;

namespace order.Repository.WebImplement;

public class WebFoodItemRepository : IFoodItemRepository
{
    private readonly WebUtils _webUtils;

    public WebFoodItemRepository(IOptions<WebConfig> config)
    {
        _webUtils = new WebUtils(config.Value.MenuUrl);
    }

    public async Task<FoodItem> GetFoodItem(string menuId, int itemIdx)
    {
        var endPoint = $"/{menuId}/foodItem/{itemIdx}";

        try
        {
            var apiResponse = await _webUtils.GetAsync<FoodItem>(endPoint);

            if (apiResponse is { Result: true })
                return apiResponse.Data;

            throw new FoodItemNotFoundException();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task AdjustFoodItemStock(string menuId, int itemIndex, int quantity)
    {
        var endPoint = $"/{menuId}/foodItem/{itemIndex}/{-quantity}";

        try
        {
            var apiResponse = await _webUtils.PostAsync<string>(endPoint);

            if (apiResponse is { Result: false })
                throw new FoodItemNotFoundException();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}