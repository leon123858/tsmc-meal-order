using order.DTO;
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

    public IEnumerable<Order> GetOrders(User user)
    {
        return _orderRepository.GetOrders(user.Id);
    }

    public Order GetOrder(User user, Guid orderId)
    {
        return _orderRepository.GetOrder(user.Id, orderId);
    }

    public Order CreateOrder(User user, OrderDTO order)
    {
        var newOrder = new Order
        {
            Id = Guid.NewGuid(),
            Customer = user,
            Restaurant = new User { Id = order.RestaurantId },
            FoodItems = order.FoodItems,
        };
        
        _orderRepository.CreateOrder(user.Id, newOrder);

        return newOrder;
    }

    public void ConfirmOrder(User user, Guid orderId)
    {
        var order = _orderRepository.GetOrder(user.Id, orderId);
        order.Confirm();
        
        _orderRepository.UpdateOrder(order);
    }

    public void DeleteOrder(User user, Guid orderId)
    {
        _orderRepository.DeleteOrder(user.Id, orderId);
    }
}