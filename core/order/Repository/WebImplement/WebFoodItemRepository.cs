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

    public async Task<FoodItem> GetFoodItem(Guid menuId, int itemIdx)
    {
        var endPoint = $"/api/menu/{menuId}/foodItem/{itemIdx}";

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
}