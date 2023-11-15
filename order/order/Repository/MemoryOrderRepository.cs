using order.Exceptions;
using order.Model;

namespace order.Repository;

public class MemoryOrderRepository : IOrderRepository
{
    private readonly List<Order> _orders = new();

    public MemoryOrderRepository()
    {
        var meal1 = new FoodItem { Id = 1, Name = "Burger", Price = 9.99m, Description = "Juicy beef burger with cheese and veggies" };
        var meal2 = new FoodItem { Id = 2, Name = "Pizza", Price = 12.99m, Description = "Delicious pizza with assorted toppings" };
        var meal3 = new FoodItem { Id = 3, Name = "Salad", Price = 6.99m, Description = "Fresh garden salad with dressing" };

        var order1 = new Order
        {
            Id = 1,
            CustomerName = "John Doe",
            FoodItems = new List<FoodItem> { meal1, meal2 },
            IsConfirmed = true
        };

        var order2 = new Order
        {
            Id = 2,
            CustomerName = "Jane Smith",
            FoodItems = new List<FoodItem> { meal3 },
            IsConfirmed = false
        };

        _orders.Add(order1);
        _orders.Add(order2);
    }

    public IEnumerable<Order> GetOrders()
    {
        return _orders;
    }

    public Order GetOrder(int id)
    {
        var order = _orders.FirstOrDefault(o => o.Id == id);

        if (order == null)
            throw new OrderNotFoundException();

        return order;
    }

    public void CreateOrder(Order order)
    {
        order.Id = _orders.Count + 1;
        _orders.Add(order);
    }

    public void UpdateOrder(int id, Order updatedOrder)
    {
        var existingOrder = _orders.FirstOrDefault(o => o.Id == id);
        if (existingOrder != null)
        {
            // TODO: Update data
        }
    }

    public void DeleteOrder(int id)
    {
        var order = _orders.FirstOrDefault(o => o.Id == id);

        if (order == null)
            throw new OrderNotFoundException();

        _orders.Remove(order);
    }

    public void ConfirmOrder(int id)
    {
        var order = _orders.FirstOrDefault(o => o.Id == id);

        if (order == null)
            throw new OrderNotFoundException();

        order.Confirm();
    }
}