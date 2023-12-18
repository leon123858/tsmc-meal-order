using core.Model;
using order.Exceptions;
using order.Model;

namespace order.Repository.TestImplement;

public class MemoryOrderRepository : IOrderRepository
{
    private readonly List<Order> _orders = new();

    public MemoryOrderRepository()
    {
        var fakeUser = new User { Id = "Kkyb8oszawYXhfP6GpTNRb711F02" };

        var meal1 = new OrderedFoodItem(new FoodItem
            { Name = "Burger", Price = 100, Description = "Juicy beef burger with cheese and veggies" }, 0, 1, "");
        var meal2 = new OrderedFoodItem(new FoodItem
            { Name = "Pizza", Price = 120, Description = "Delicious pizza with assorted toppings" }, 0, 2, "");
        var meal3 = new OrderedFoodItem(new FoodItem
            { Name = "Salad", Price = 130, Description = "Fresh garden salad with dressing" }, 0, 1, "");

        var order1 = new Order
        {
            Id = Guid.NewGuid(),
            Customer = fakeUser,
            Restaurant = new User { Id = "4bCkldMFxoh5kP9byf7GUFsiF2t2" },
            FoodItems = new List<OrderedFoodItem> { meal1, meal2 }
        };

        var order2 = new Order
        {
            Id = Guid.NewGuid(),
            Customer = fakeUser,
            Restaurant = new User { Id = "4bCkldMFxoh5kP9byf7GUFsiF2t2" },
            FoodItems = new List<OrderedFoodItem> { meal3 }
        };

        _orders.Add(order1);
        _orders.Add(order2);
    }

    public Task<IEnumerable<Order>> GetOrders(string userId)
    {
        return Task.FromResult(_orders.Where(_ => _.Customer.Id == userId));
    }

    public Task<IEnumerable<Order>> GetOrdersByRestaurant(string restaurantId)
    {
        throw new NotImplementedException();
    }

    public Task<Order> GetOrder(Guid orderId)
    {
        var order = _orders.FirstOrDefault(_ => _.Id == orderId);

        if (order == null)
            throw new OrderNotFoundException();

        return Task.FromResult(order);
    }

    public Task CreateOrder(Order order)
    {
        _orders.Add(order);
        return Task.CompletedTask;
    }

    public Task UpdateOrder(Order order)
    {
        var index = _orders.FindIndex(_ => _.Id == order.Id);

        if (index == -1)
            throw new OrderNotFoundException();

        _orders[index] = order;

        return Task.CompletedTask;
    }

    public Task<IEnumerable<Order>> GetOrdersByDate(DateTime date)
    {
        return Task.FromResult(_orders.Where(_ => _.OrderDate.Date == date.Date));
    }
}