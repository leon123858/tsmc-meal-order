using System.Transactions;
using order.DTO.Web;
using order.Model;
using order.Repository;

namespace order.Service;

public class OrderService
{
    private readonly IFoodItemRepository _foodItemRepository;
    private readonly IMailService _mailService;
    private readonly IOrderRepository _orderRepository;
    private readonly IUserRepository _userRepository;

    public OrderService(IOrderRepository orderRepository, IFoodItemRepository foodItemRepository,
        IUserRepository userRepository,
        IMailService mailService)
    {
        _orderRepository = orderRepository;
        _foodItemRepository = foodItemRepository;
        _userRepository = userRepository;
        _mailService = mailService;
    }

    public async Task<IEnumerable<Order>> GetOrders(User user)
    {
        var ordersEnumerable = user.Type == UserType.admin
            ? await _orderRepository.GetOrdersByRestaurant(user.Id)
            : await _orderRepository.GetOrders(user.Id);

        var orders = ordersEnumerable.ToList();

        var userIds = orders.SelectMany(_ => new[] { _.Customer.Id, _.Restaurant.Id }).Distinct();
        var userDictionary = await _userRepository.GetUsers(userIds);

        foreach (var order in orders)
        {
            order.Customer = userDictionary[order.Customer.Id];
            order.Restaurant = userDictionary[order.Restaurant.Id];
        }

        return orders;
    }

    public async Task<Order> GetOrder(User user, Guid orderId)
    {
        var order = await _orderRepository.GetOrder(orderId);

        if (user.Type == UserType.admin && order.Restaurant.Id != user.Id)
            throw new Exception("User is not the owner of the order");

        if (user.Type == UserType.normal && order.Customer.Id != user.Id)
            throw new Exception("User is not the owner of the order");

        var userIds = new[] { order.Customer.Id, order.Restaurant.Id }.Distinct();
        var userDictionary = await _userRepository.GetUsers(userIds);

        order.Customer = userDictionary[order.Customer.Id];
        order.Restaurant = userDictionary[order.Restaurant.Id];

        return order;
    }

    public async Task<Order> CreateOrder(User user, User restaurant, CreateOrderWebDTO createOrderWeb)
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        var foodItem = await _foodItemRepository.GetFoodItem(restaurant.Id, createOrderWeb.FoodItemId);

        if (foodItem.Count < createOrderWeb.Count)
            throw new Exception("Not enough food items in stock");

        var orderedFoodItem = new OrderedFoodItem(foodItem, createOrderWeb.FoodItemId, createOrderWeb.Count,
            createOrderWeb.Description);

        var newOrder = new Order
        {
            Id = Guid.NewGuid(),
            Customer = user,
            Restaurant = restaurant,
            OrderDate = createOrderWeb.OrderDate,
            MealType = Enum.Parse<MealType>(createOrderWeb.MealType),
            FoodItems = new List<OrderedFoodItem> { orderedFoodItem }
        };

        await _orderRepository.CreateOrder(newOrder);

        foreach (var item in newOrder.FoodItems)
            await _foodItemRepository.AdjustFoodItemStock(restaurant.Id, item.Index, -item.Count);

        scope.Complete();

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

    public async Task NotifyCustomers(MealType mealType)
    {
        var orders = (await _orderRepository.GetOrdersByDate(DateTime.Today)).ToList();
        var ordersInMealType = orders.Where(_ => _.MealType == mealType && _.Status == OrderStatus.Preparing).ToList();
        
        var userIds = ordersInMealType.Select(_ => _.Customer.Id).Distinct();
        var userDictionary = await _userRepository.GetUsers(userIds);

        foreach (var order in ordersInMealType)
        {
            order.Customer = userDictionary[order.Customer.Id];
            _mailService.SendOrderNotifyMail(order);
        }
    }
}