using core.Model;
using order.DTO.Web;
using order.Model;
using order.Repository;

namespace order.Service;

public class OrderService
{
    private readonly IFoodItemRepository _foodItemRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly MailService _mailService;

    public OrderService(IOrderRepository orderRepository, IFoodItemRepository foodItemRepository,
        MailService mailService)
    {
        _orderRepository = orderRepository;
        _foodItemRepository = foodItemRepository;
        _mailService = mailService;
    }

    public async Task<IEnumerable<Order>> GetOrders(User user)
    {
        if (user.Type == UserType.admin)
            return await _orderRepository.GetOrdersByRestaurant(user.Id);

        return await _orderRepository.GetOrders(user.Id);
    }

    public async Task<Order> GetOrder(User user, Guid orderId)
    {
        var order = await _orderRepository.GetOrder(orderId);

        if (user.Type == UserType.admin && order.Restaurant.Id != user.Id)
            throw new Exception("User is not the owner of the order");
        
        if (user.Type == UserType.normal && order.Customer.Id != user.Id)
            throw new Exception("User is not the owner of the order");

        return order;
    }

    public async Task<Order> CreateOrder(User user, User restaurant, CreateOrderWebDTO createOrderWeb)
    {
        var foodItem = await _foodItemRepository.GetFoodItem(restaurant.Id, createOrderWeb.FoodItemId);
        
        if (foodItem.Count < createOrderWeb.Count)
            throw new Exception("Not enough food items in stock");
        
        var orderedFoodItem = new OrderedFoodItem(foodItem, createOrderWeb.FoodItemId, createOrderWeb.Count, createOrderWeb.Description);

        var newOrder = new Order
        {
            Id = Guid.NewGuid(),
            Customer = user,
            Restaurant = restaurant,
            OrderDate = createOrderWeb.OrderDate,
            MealType = Enum.Parse<MealType>(createOrderWeb.MealType),
            FoodItems = new List<OrderedFoodItem> { orderedFoodItem },
        };

        await _orderRepository.CreateOrder(newOrder);
        
        foreach (var item in newOrder.FoodItems)
            await _foodItemRepository.AdjustFoodItemStock(restaurant.Id, item.Index, -item.Count);
        
        _mailService.SendOrderCreatedMail(newOrder);

        return newOrder;
    }

    public async Task ConfirmOrder(User restaurant, Guid orderId)
    {
        var order = await _orderRepository.GetOrder(orderId);

        if (order.Restaurant.Id != restaurant.Id)
            throw new Exception("User is not the owner of the order");

        order.Confirm();

        await _orderRepository.UpdateOrder(order);

        _mailService.SendOrderConfirmedMail(order);
    }

    public async Task DeleteOrder(User user, Guid orderId)
    {
        var order = await _orderRepository.GetOrder(orderId);

        if (user.Type == UserType.admin && order.Restaurant.Id != user.Id)
            throw new Exception("User is not the owner of the order");
        
        if (user.Type == UserType.normal && order.Customer.Id != user.Id)
            throw new Exception("User is not the owner of the order");

        order.Cancel();

        await _orderRepository.UpdateOrder(order);

        foreach (var foodItem in order.FoodItems)
            await _foodItemRepository.AdjustFoodItemStock(order.Restaurant.Id, foodItem.Index, foodItem.Count);

        _mailService.SendOrderDeletedMail(order);
    }
}