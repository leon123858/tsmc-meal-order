using core.Model;
using NSubstitute;
using order.DTO.Web;
using order.Model;
using order.Repository;
using order.Service;
using User = order.Model.User;

namespace core_test;

[TestFixture]
public class OrderServiceTest
{
    [SetUp]
    public void SetUp()
    {
        _orderRepository = Substitute.For<IOrderRepository>();
        _foodItemRepository = Substitute.For<IFoodItemRepository>();
        _mailService = Substitute.For<IMailService>();

        _orderService = new OrderService(_orderRepository, _foodItemRepository, _mailService);
    }

    private IOrderRepository _orderRepository;
    private IFoodItemRepository _foodItemRepository;
    private IMailService _mailService;

    private OrderService _orderService;

    [Test]
    public async Task GetOrders_AdminUser_ReturnsOrdersByRestaurant()
    {
        // Arrange
        var adminUser = new User { Id = "Test", Type = UserType.admin };
        _orderRepository.GetOrdersByRestaurant(adminUser.Id).Returns(new List<Order>());

        // Act
        var result = await _orderService.GetOrders(adminUser);

        // Assert
        Assert.IsNotNull(result);
        await _orderRepository.Received().GetOrdersByRestaurant(adminUser.Id);
    }

    [Test]
    public async Task GetOrders_NormalUser_ReturnsOrders()
    {
        // Arrange
        var normalUser = new User { Id = "Test", Type = UserType.normal };
        _orderRepository.GetOrders(normalUser.Id).Returns(new List<Order>());

        // Act
        var result = await _orderService.GetOrders(normalUser);

        // Assert
        Assert.IsNotNull(result);
        await _orderRepository.Received().GetOrders(Arg.Any<string>());
    }

    [Test]
    public async Task GetOrder_AdminUser_ReturnsOrderForRestaurant()
    {
        // Arrange
        var adminUser = new User { Id = "Test", Type = UserType.admin };
        var orderId = Guid.NewGuid();
        var order = new Order { Id = orderId, Restaurant = new User { Id = adminUser.Id } };
        _orderRepository.GetOrder(orderId).Returns(order);

        // Act
        var result = await _orderService.GetOrder(adminUser, orderId);

        // Assert
        Assert.IsNotNull(result);
        await _orderRepository.Received().GetOrder(orderId);
    }

    [Test]
    public async Task GetOrder_NormalUser_ReturnsOrderForCustomer()
    {
        // Arrange
        var normalUser = new User { Id = "Test", Type = UserType.normal };
        var orderId = Guid.NewGuid();
        var order = new Order { Id = orderId, Customer = new User { Id = normalUser.Id } };
        _orderRepository.GetOrder(orderId).Returns(order);

        // Act
        var result = await _orderService.GetOrder(normalUser, orderId);

        // Assert
        Assert.IsNotNull(result);
        await _orderRepository.Received().GetOrder(orderId);
    }

    [Test]
    public void GetOrder_AdminUser_OrderBelongsToDifferentRestaurant_ThrowsException()
    {
        // Arrange
        var adminUser = new User { Id = "Test", Type = UserType.admin };
        var orderId = Guid.NewGuid();
        var order = new Order { Id = orderId, Restaurant = new User { Id = "Test2" } };
        _orderRepository.GetOrder(orderId).Returns(order);

        // Act & Assert
        Assert.ThrowsAsync<Exception>(async () => await _orderService.GetOrder(adminUser, orderId));
    }

    [Test]
    public void GetOrder_NormalUser_OrderDoesNotBelongToUser_ThrowsException()
    {
        // Arrange
        var normalUser = new User { Id = "Test", Type = UserType.normal };
        var orderId = Guid.NewGuid();
        var order = new Order { Id = orderId, Customer = new User { Id = "Test2" } };
        _orderRepository.GetOrder(orderId).Returns(order);

        // Act & Assert
        Assert.ThrowsAsync<Exception>(async () => await _orderService.GetOrder(normalUser, orderId));
    }

    [Test]
    public async Task ConfirmOrder_ValidRestaurant_OrderStatusIsPreparing()
    {
        // Arrange
        var restaurant = new User { Id = "Test" };
        var orderId = Guid.NewGuid();
        var order = new Order { Id = orderId, Restaurant = new User { Id = restaurant.Id } };
        _orderRepository.GetOrder(orderId).Returns(order);

        // Act
        await _orderService.ConfirmOrder(restaurant, orderId);

        // Assert
        Assert.That(order.Status, Is.EqualTo(OrderStatus.Preparing));
    }

    [Test]
    public async Task ConfirmOrder_ValidRestaurant_OrderIsUpdated()
    {
        // Arrange
        var restaurant = new User { Id = "Test" };
        var orderId = Guid.NewGuid();
        var order = new Order { Id = orderId, Restaurant = new User { Id = restaurant.Id } };
        _orderRepository.GetOrder(orderId).Returns(order);

        // Act
        await _orderService.ConfirmOrder(restaurant, orderId);

        // Assert
        await _orderRepository.Received().UpdateOrder(Arg.Any<Order>());
    }

    [Test]
    public async Task ConfirmOrder_ValidRestaurant_MailIsSent()
    {
        // Arrange
        var restaurant = new User { Id = "Test" };
        var orderId = Guid.NewGuid();
        var order = new Order { Id = orderId, Restaurant = new User { Id = restaurant.Id } };
        _orderRepository.GetOrder(orderId).Returns(order);

        // Act
        await _orderService.ConfirmOrder(restaurant, orderId);

        // Assert
        _mailService.Received().SendOrderConfirmedMail(Arg.Any<Order>());
    }

    [Test]
    public void ConfirmOrder_InvalidRestaurant_ThrowsException()
    {
        // Arrange
        var invalidRestaurant = new User { Id = "Test" };
        var orderId = Guid.NewGuid();
        var order = new Order { Id = orderId, Restaurant = new User { Id = "Test1" } };
        _orderRepository.GetOrder(orderId).Returns(order);

        // Act & Assert
        Assert.ThrowsAsync<Exception>(async () => await _orderService.ConfirmOrder(invalidRestaurant, orderId));
    }

    [Test]
    public void ConfirmOrder_OrderAlreadyConfirmed_ThrowsException()
    {
        // Arrange
        var restaurant = new User { Id = "Test" };
        var orderId = Guid.NewGuid();
        var order = new Order
            { Id = orderId, Restaurant = new User { Id = restaurant.Id }, Status = OrderStatus.Preparing };
        _orderRepository.GetOrder(orderId).Returns(order);

        // Act & Assert
        Assert.ThrowsAsync<Exception>(async () => await _orderService.ConfirmOrder(restaurant, orderId));
    }

    [Test]
    public async Task DeleteOrder_ValidUser_OrderStatusIsCanceled()
    {
        // Arrange
        var user = new User { Id = "Test" };
        var orderId = Guid.NewGuid();
        var order = new Order { Id = orderId, Customer = new User { Id = user.Id } };
        _orderRepository.GetOrder(orderId).Returns(order);

        // Act
        await _orderService.DeleteOrder(user, orderId);

        // Assert
        Assert.That(order.Status, Is.EqualTo(OrderStatus.Canceled));
    }

    [Test]
    public async Task DeleteOrder_ValidUser_OrderIsUpdated()
    {
        // Arrange
        var user = new User { Id = "Test" };
        var orderId = Guid.NewGuid();
        var order = new Order { Id = orderId, Customer = new User { Id = user.Id } };
        _orderRepository.GetOrder(orderId).Returns(order);

        // Act
        await _orderService.DeleteOrder(user, orderId);

        // Assert
        await _orderRepository.Received().UpdateOrder(Arg.Any<Order>());
    }

    [Test]
    public async Task DeleteOrder_ValidUser_MailIsSent()
    {
        // Arrange
        var user = new User { Id = "Test" };
        var orderId = Guid.NewGuid();
        var order = new Order { Id = orderId, Customer = new User { Id = user.Id } };
        _orderRepository.GetOrder(orderId).Returns(order);

        // Act
        await _orderService.DeleteOrder(user, orderId);

        // Assert
        _mailService.Received().SendOrderDeletedMail(Arg.Any<Order>());
    }

    [Test]
    public async Task DeleteOrder_ValidUser_StockIsUpdated()
    {
        // Arrange
        var user = new User { Id = "Test" };
        var orderId = Guid.NewGuid();
        var order = new Order
        {
            Id = orderId, Customer = new User { Id = user.Id }, Restaurant = new User { Id = "Test2" }, FoodItems =
                new List<OrderedFoodItem>
                {
                    new(new FoodItem(), 0, 1, "Test")
                }
        };
        _orderRepository.GetOrder(orderId).Returns(order);

        // Act
        await _orderService.DeleteOrder(user, orderId);

        // Assert
        await _foodItemRepository.Received().AdjustFoodItemStock(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<int>());
    }

    [Test]
    public void DeleteOrder_InvalidUser_ThrowsException()
    {
        // Arrange
        var invalidUser = new User { Id = "Test" };
        var orderId = Guid.NewGuid();
        var order = new Order { Id = orderId, Customer = new User { Id = "Test2" } };
        _orderRepository.GetOrder(orderId).Returns(order);

        // Act & Assert
        Assert.ThrowsAsync<Exception>(async () => await _orderService.DeleteOrder(invalidUser, orderId));
    }

    [Test]
    public void DeleteOrder_InvalidRestaurant_ThrowsException()
    {
        // Arrange
        var invalidRestaurant = new User { Id = "Test", Type = UserType.admin };
        var orderId = Guid.NewGuid();
        var order = new Order { Id = orderId, Restaurant = new User { Id = "Test2" } };
        _orderRepository.GetOrder(orderId).Returns(order);

        // Act & Assert
        Assert.ThrowsAsync<Exception>(async () => await _orderService.DeleteOrder(invalidRestaurant, orderId));
    }

    [Test]
    public void DeleteOrder_OrderAlreadyCancelled_ThrowsException()
    {
        // Arrange
        var user = new User { Id = "Test" };
        var orderId = Guid.NewGuid();
        var order = new Order { Id = orderId, Customer = new User { Id = user.Id }, Status = OrderStatus.Canceled };
        _orderRepository.GetOrder(orderId).Returns(order);

        // Act & Assert
        Assert.ThrowsAsync<Exception>(async () => await _orderService.DeleteOrder(user, orderId));
    }

    [Test]
    public void DeleteOrder_OrderAlreadyPreparing_ThrowsException()
    {
        // Arrange
        var user = new User { Id = "Test" };
        var orderId = Guid.NewGuid();
        var order = new Order { Id = orderId, Customer = new User { Id = user.Id }, Status = OrderStatus.Preparing };
        _orderRepository.GetOrder(orderId).Returns(order);

        // Act & Assert
        Assert.ThrowsAsync<Exception>(async () => await _orderService.DeleteOrder(user, orderId));
    }
    
    [Test]
    public async Task CreateOrder_WithValidData_CreatesOrder()
    {
        // Arrange
        var user = new User { Id = "Test" };
        var restaurant = new User { Id = "Test1" };
        var createOrderWebDto = new CreateOrderWebDTO
        {
            MenuId = restaurant.Id,
            FoodItemId = 1,
            Count = 2,
            OrderDate = DateTime.UtcNow,
            MealType = "Lunch",
            Description = "Special Instructions"
        };
    
        var foodItem = new FoodItem { Count = 5 };
        _foodItemRepository.GetFoodItem(restaurant.Id, createOrderWebDto.FoodItemId).Returns(foodItem);

        // Act
        await _orderService.CreateOrder(user, restaurant, createOrderWebDto);

        // Assert
        await _orderRepository.Received().CreateOrder(Arg.Any<Order>());
    }
    
    [Test]
    public async Task CreateOrder_WithValidData_MailIsSent()
    {
        // Arrange
        var user = new User { Id = "Test" };
        var restaurant = new User { Id = "Test1" };
        var createOrderWebDto = new CreateOrderWebDTO
        {
            MenuId = restaurant.Id,
            FoodItemId = 1,
            Count = 2,
            OrderDate = DateTime.UtcNow,
            MealType = "Lunch",
            Description = "Special Instructions"
        };
    
        var foodItem = new FoodItem { Count = 5 };
        _foodItemRepository.GetFoodItem(restaurant.Id, createOrderWebDto.FoodItemId).Returns(foodItem);

        // Act
        await _orderService.CreateOrder(user, restaurant, createOrderWebDto);

        // Assert
        _mailService.Received().SendOrderCreatedMail(Arg.Any<Order>());
    }
    
    [Test]
    public async Task CreateOrder_WithValidData_AdjustStock()
    {
        // Arrange
        var user = new User { Id = "Test" };
        var restaurant = new User { Id = "Test1" };
        var createOrderWebDto = new CreateOrderWebDTO
        {
            MenuId = restaurant.Id,
            FoodItemId = 1,
            Count = 2,
            OrderDate = DateTime.UtcNow,
            MealType = "Lunch",
            Description = "Special Instructions"
        };
    
        var foodItem = new FoodItem { Count = 5 };
        _foodItemRepository.GetFoodItem(restaurant.Id, createOrderWebDto.FoodItemId).Returns(foodItem);

        // Act
        await _orderService.CreateOrder(user, restaurant, createOrderWebDto);

        // Assert
        await _foodItemRepository.Received().AdjustFoodItemStock(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<int>());
    }

    [Test]
    public void CreateOrder_InsufficientStock_ThrowsException()
    {
        // Arrange
        var user = new User { Id = "Test" };
        var restaurant = new User { Id = "Test1" };
        var createOrderWebDto = new CreateOrderWebDTO
        {
            MenuId = restaurant.Id,
            FoodItemId = 1,
            Count = 2,
            OrderDate = DateTime.UtcNow,
            MealType = "Lunch",
            Description = "Special Instructions"
        };
    
        var foodItem = new FoodItem { Count = 1 };
        _foodItemRepository.GetFoodItem(restaurant.Id, createOrderWebDto.FoodItemId).Returns(foodItem);

        // Act & Assert
        Assert.ThrowsAsync<Exception>(async () => await _orderService.CreateOrder(user, restaurant, createOrderWebDto));
    }
}