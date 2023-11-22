﻿using core.Model;
using order.Exceptions;
using order.Model;

namespace order.Repository;

public class MemoryOrderRepository : IOrderRepository
{
    private readonly List<Order> _orders = new();

    public MemoryOrderRepository()
    {
        var meal1 = new FoodItem { Name = "Burger", Price = 100, Description = "Juicy beef burger with cheese and veggies" };
        var meal2 = new FoodItem { Name = "Pizza", Price = 120, Description = "Delicious pizza with assorted toppings" };
        var meal3 = new FoodItem { Name = "Salad", Price = 130, Description = "Fresh garden salad with dressing" };

        var order1 = new Order
        {
            Id = Guid.NewGuid(),
            CustomerName = "John Doe",
            FoodItems = new List<FoodItem> { meal1, meal2 },
            IsConfirmed = true
        };

        var order2 = new Order
        {
            Id = Guid.NewGuid(),
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

    public Order GetOrder(Guid id)
    {
        var order = _orders.FirstOrDefault(o => o.Id == id);

        if (order == null)
            throw new OrderNotFoundException();

        return order;
    }

    public void CreateOrder(Order order)
    {
        order.Id = Guid.NewGuid();
        _orders.Add(order);
    }

    public void UpdateOrder(Guid id, Order updatedOrder)
    {
        var existingOrder = _orders.FirstOrDefault(o => o.Id == id);
        if (existingOrder != null)
        {
            // TODO: Update data
        }
    }

    public void DeleteOrder(Guid id)
    {
        var order = _orders.FirstOrDefault(o => o.Id == id);

        if (order == null)
            throw new OrderNotFoundException();

        _orders.Remove(order);
    }

    public void ConfirmOrder(Guid id)
    {
        var order = _orders.FirstOrDefault(o => o.Id == id);

        if (order == null)
            throw new OrderNotFoundException();

        order.Confirm();
    }
}