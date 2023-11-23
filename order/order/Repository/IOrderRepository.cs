using order.DTO;
using order.Model;

namespace order.Repository;

public interface IOrderRepository
{
    IEnumerable<Order> GetOrders(Guid userId);
    Order GetOrder(Guid userId, Guid orderId);
    void CreateOrder(Guid userId, Order order);
    void DeleteOrder(Guid userId, Guid orderId);
    void UpdateOrder(Order order);
}