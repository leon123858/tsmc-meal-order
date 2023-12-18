using order.Model;

namespace order.Repository;

public interface IOrderRepository
{
    Task<IEnumerable<Order>> GetOrders(string userId);
    Task<IEnumerable<Order>> GetOrdersByRestaurant(string restaurantId);
    Task<Order> GetOrder(Guid orderId);
    Task CreateOrder(Order order);
    Task UpdateOrder(Order order);
    Task<IEnumerable<Order>> GetOrdersByDate(DateTime date);
}