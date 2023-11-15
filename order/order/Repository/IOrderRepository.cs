using order.Model;

namespace order.Repository;

public interface IOrderRepository
{
    IEnumerable<Order> GetOrders();
    Order GetOrder(int id);
    void CreateOrder(Order order);
    void UpdateOrder(int id, Order updatedOrder);
    void DeleteOrder(int id);
    void ConfirmOrder(int id);
}