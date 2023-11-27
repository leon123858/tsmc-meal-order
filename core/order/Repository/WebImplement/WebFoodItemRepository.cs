using core.Model;
using order.Exceptions;

namespace order.Repository.WebImplement;

public class WebFoodItemRepository : IFoodItemRepository
{
    private const string Url = "http://localhost:5182";
    private readonly WebUtils _webUtils;

    public WebFoodItemRepository()
    {
        _webUtils = new WebUtils(Url);
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