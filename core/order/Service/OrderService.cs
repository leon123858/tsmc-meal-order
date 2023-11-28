using core.Model;
using order.DTO.Web;
using order.Model;
using order.Repository;

namespace order.Service;

public class OrderService
{
    private readonly IFoodItemRepository _foodItemRepository;
    private readonly IOrderRepository _orderRepository;

    public OrderService(IOrderRepository orderRepository, IFoodItemRepository foodItemRepository)
    {
        _orderRepository = orderRepository;
        _foodItemRepository = foodItemRepository;
    }

    public async Task<IEnumerable<Order>> GetOrders(User user)
    {
        return await _orderRepository.GetOrders(user.Id);
    }

    public async Task<Order> GetOrder(User user, Guid orderId)
    {
        return await _orderRepository.GetOrder(user.Id, orderId);
    }

    public async Task<Order> CreateOrder(User user, CreateOrderWebDTO createOrderWeb)
    {
        var foodItems = new List<FoodItem>();

        foreach (var foodItemId in createOrderWeb.FoodItemIds)
        {
            var foodItem = await _foodItemRepository.GetFoodItem(createOrderWeb.RestaurantId, foodItemId);
            foodItems.Add(foodItem);
        }

        var newOrder = new Order
        {
            Id = Guid.NewGuid(),
            Customer = user,
            OrderDate = createOrderWeb.OrderDate,
            Restaurant = new User { Id = createOrderWeb.RestaurantId },
            FoodItems = foodItems
        };

        await _orderRepository.CreateOrder(newOrder);

        return newOrder;
    }

    public async Task ConfirmOrder(User user, Guid orderId)
    {
        var order = await _orderRepository.GetOrder(user.Id, orderId);
        order.Confirm();

        await _orderRepository.UpdateOrder(order);
    }

    public async Task DeleteOrder(User user, Guid orderId)
    {
        var order = await _orderRepository.GetOrder(user.Id, orderId);
        order.Cancel();

        await _orderRepository.UpdateOrder(order);
    }
}