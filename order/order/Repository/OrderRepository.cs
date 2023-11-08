using order.Model;

namespace order.Repository
{
    public class OrderRepository
    {
        private List<Order> orders = new();

        public IEnumerable<Order> GetOrders()
        {
            return orders;
        }

        public Order? GetOrder(Guid id)
        {
            return orders.FirstOrDefault(o => o.Id == id);
        }

        public void CreateOrder(Order order)
        {
            order.Id = Guid.NewGuid();
            orders.Add(order);
        }

        public void UpdateOrder(Guid id, Order updatedOrder)
        {
            var existingOrder = orders.FirstOrDefault(o => o.Id == id);
            if (existingOrder != null)
            {
                // TODO: update data
            }
        }

        public void DeleteOrder(Guid id)
        {
            var order = orders.FirstOrDefault(o => o.Id == id);
            if (order != null) orders.Remove(order);
        }


        public void ConfirmOrder(Guid id)
        {
            var order = orders.FirstOrDefault(o => o.Id == id);
            order?.Confirm();
        }
    }
}
