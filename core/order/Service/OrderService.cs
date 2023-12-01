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

    public OrderService(IOrderRepository orderRepository, IFoodItemRepository foodItemRepository, MailService mailService)
    {
        _orderRepository = orderRepository;
        _foodItemRepository = foodItemRepository;
        _mailService = mailService;
    }

    public async Task<IEnumerable<Order>> GetOrders(User user)
    {
        return await _orderRepository.GetOrders(user.Id);
    }

    public async Task<Order> GetOrder(User user, Guid orderId)
    {
        var order = await _orderRepository.GetOrder(orderId);
        
        if (order.Customer.Id != user.Id)
            throw new Exception("User is not the owner of the order");
        
        return order;
    }

    public async Task<Order> CreateOrder(User user, CreateOrderWebDTO createOrderWeb)
    {
        var foodItems = new List<FoodItem>();

        foreach (var foodItemId in createOrderWeb.FoodItemIds)
        {
            var foodItem = await _foodItemRepository.GetFoodItem(createOrderWeb.MenuId, foodItemId);
            foodItems.Add(foodItem);
        }

        var newOrder = new Order
        {
            Id = Guid.NewGuid(),
            Customer = user,
            // TODO get restaurant from menu
            Restaurant = new User() { Id = "4bCkldMFxoh5kP9byf7GUFsiF2t2" },
            OrderDate = createOrderWeb.OrderDate,
            FoodItems = foodItems
        };

        await _orderRepository.CreateOrder(newOrder);
        
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
        
        if (order.Customer.Id != user.Id)
            throw new Exception("User is not the owner of the order");
        
        order.Cancel();

        await _orderRepository.UpdateOrder(order);
        
        _mailService.SendOrderDeletedMail(order);
    }
}