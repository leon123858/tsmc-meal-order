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

        public IEnumerable<Order> GetOrders()
        {
            throw new NotImplementedException();
        }

        public Order GetOrder(Guid id)
        {
            throw new NotImplementedException();
        }

        public void CreateOrder(Order order)
        {
            throw new NotImplementedException();
        }

        public void UpdateOrder(Guid id, Order updatedOrder)
        {
            throw new NotImplementedException();
        }

        public void DeleteOrder(Guid id)
        {
            throw new NotImplementedException();
        }

        public void ConfirmOrder(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
