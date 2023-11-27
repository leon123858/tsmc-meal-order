﻿using order.DTO;
using order.Model;

namespace order.Repository;

public interface IOrderRepository
{
    // change all the methods to async
    Task<IEnumerable<Order>> GetOrders(Guid userId);
    Task<Order> GetOrder(Guid userId, Guid orderId);
    Task CreateOrder(Guid userId, Order order);
    Task DeleteOrder(Guid userId, Guid orderId);
    Task UpdateOrder(Order order);
}