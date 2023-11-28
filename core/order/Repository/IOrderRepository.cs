using order.Model;

namespace order.Repository;

public interface IOrderRepository
{
    Task<IEnumerable<Order>> GetOrders(Guid userId);
    Task<Order> GetOrder(Guid userId, Guid orderId);
    Task CreateOrder(Order order);
    Task UpdateOrder(Order order);
}