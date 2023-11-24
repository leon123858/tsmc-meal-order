using System.Data;
using System.Data.SqlClient;
using order.Model;

namespace order.Repository
{
    public class SqlOrderRepository : IOrderRepository
    {
        private const string _connectionString = "";

        private IDbConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public IEnumerable<Order> GetOrders(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Order GetOrder(Guid userId, Guid orderId)
        {
            throw new NotImplementedException();
        }

        public void CreateOrder(Guid userId, Order order)
        {
            throw new NotImplementedException();
        }

        public void UpdateOrder(Guid id, Order updatedOrder)
        {
            throw new NotImplementedException();
        }

        public void DeleteOrder(Guid userId, Guid orderId)
        {
            throw new NotImplementedException();
        }

        public void UpdateOrder(Order order)
        {
            throw new NotImplementedException();
        }
    }
}
