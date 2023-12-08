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

        var meal1 = new FoodItem
            { Name = "Burger", Price = 100, Description = "Juicy beef burger with cheese and veggies" };
        var meal2 = new FoodItem
            { Name = "Pizza", Price = 120, Description = "Delicious pizza with assorted toppings" };
        var meal3 = new FoodItem { Name = "Salad", Price = 130, Description = "Fresh garden salad with dressing" };

        var order1 = new Order
        {
            Id = Guid.NewGuid(),
            Customer = fakeUser,
            Restaurant = new User { Id = "4bCkldMFxoh5kP9byf7GUFsiF2t2" },
            FoodItems = new List<FoodItem> { meal1, meal2 }
        };

        var order2 = new Order
        {
            Id = Guid.NewGuid(),
            Customer = fakeUser,
            Restaurant = new User { Id = "4bCkldMFxoh5kP9byf7GUFsiF2t2" },
            FoodItems = new List<FoodItem> { meal3 }
        };

        _orders.Add(order1);
        _orders.Add(order2);
    }

    public Task<IEnumerable<Order>> GetOrders(string userId)
    {
        return Task.FromResult(_orders.Where(_ => _.Customer.Id == userId));
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
}