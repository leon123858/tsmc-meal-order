using order.Model;
using order.Repository;

namespace order.Service;

public class OrderService
{
    private readonly IOrderRepository _orderRepository;

    public OrderService(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public IEnumerable<Order> GetOrders()
    {
        return _orderRepository.GetOrders();
    }

    public Order GetOrder(int id)
    {
        return _orderRepository.GetOrder(id);
    }

    public void CreateOrder(Order order)
    {
        _orderRepository.CreateOrder(order);
    }

    public void UpdateOrder(int id, Order updatedOrder)
    {
        _orderRepository.UpdateOrder(id, updatedOrder);
    }

    public void ConfirmOrder(int id)
    {
        _orderRepository.ConfirmOrder(id);
    }

    public void DeleteOrder(int id)
    {
        _orderRepository.DeleteOrder(id);
    }
}