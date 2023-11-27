using System.Data;
using System.Data.SqlClient;
using order.Model;

namespace order.Repository.SqlImplement;

public class SqlOrderRepository : IOrderRepository
{
    private const string _connectionString = "";

    public Task<IEnumerable<Order>> GetOrders(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task<Order> GetOrder(Guid userId, Guid orderId)
    {
        throw new NotImplementedException();
    }

    public Task CreateOrder(Guid userId, Order order)
    {
        throw new NotImplementedException();
    }

    public Task DeleteOrder(Guid userId, Guid orderId)
    {
        throw new NotImplementedException();
    }

    public Task UpdateOrder(Order order)
    {
        throw new NotImplementedException();
    }

    private IDbConnection GetConnection()
    {
        return new SqlConnection(_connectionString);
    }

    public void UpdateOrder(Guid id, Order updatedOrder)
    {
        throw new NotImplementedException();
    }
}