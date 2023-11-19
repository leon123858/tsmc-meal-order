using order.Model;

namespace order.Repository;

public interface IOrderRepository
{
    IEnumerable<Order> GetOrders();
    Order GetOrder(Guid id);
    void CreateOrder(Order order);
    void UpdateOrder(Guid id, Order updatedOrder);
    void DeleteOrder(Guid id);
    void ConfirmOrder(Guid id);
}