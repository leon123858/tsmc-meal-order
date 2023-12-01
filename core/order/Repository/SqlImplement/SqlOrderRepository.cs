using System.Data.SqlClient;
using core.Model;
using Dapper;
using Microsoft.Extensions.Options;
using order.Config;
using order.DTO.Sql;
using order.Model;

namespace order.Repository.SqlImplement;

public class SqlOrderRepository : IOrderRepository
{
    private readonly string _connectionString;
    private readonly ILogger<SqlOrderRepository> _logger;

    public SqlOrderRepository(IOptions<DatabaseConfig> config, ILogger<SqlOrderRepository> logger)
    {
        _logger = logger;
        var secretPassword = Environment.GetEnvironmentVariable("SQL_PASSWORD") ?? config.Value.Password;
        _connectionString =
            $"Persist Security Info=False;User ID={config.Value.UserName};Password={secretPassword};Server={config.Value.Host};Database={config.Value.DatabaseName};";
    }

    public async Task<IEnumerable<Order>> GetOrders(string userId)
    {
        var sql =
            @$"SELECT o.*, f.* FROM [order] AS o LEFT JOIN foodItem AS f on o.Id = f.OrderId WHERE o.UserId = '{userId}'";
        return await GetOrderImp(sql);
    }

    public async Task<Order> GetOrder(Guid orderId)
    {
        var sql =
            @$"SELECT o.*, f.* FROM [order] AS o LEFT JOIN foodItem AS f on o.Id = f.OrderId WHERE o.Id = '{orderId}'";
        return (await GetOrderImp(sql)).First();
    }

    public async Task CreateOrder(Order order)
    {
        await using var conn = new SqlConnection(_connectionString);
        conn.Open();
        await using var transaction = conn.BeginTransaction();

        var orderDto = (OrderSqlDTO)order;

        var foodItemDtos = new List<FoodItemSqlDTO>();
        foreach (var foodItem in order.FoodItems)
        {
            var foodItemDto = (FoodItemSqlDTO)foodItem;
            foodItemDto.OrderId = order.Id;
            foodItemDtos.Add(foodItemDto);
        }

        try
        {
            await conn.ExecuteInsertAsync(orderDto, transaction);
            await conn.ExecuteInsertAsync(foodItemDtos, transaction);
            
            transaction.Commit();
        }
        catch (Exception e)
        {
            _logger.LogError("Create order failed, Exception: {Message}", e.Message);
            transaction.Rollback();
            throw;
        }
    }

    public async Task UpdateOrder(Order order)
    {
        await using var conn = new SqlConnection(_connectionString);
        conn.Open();
        await using var transaction = conn.BeginTransaction();

        var orderDto = (OrderSqlDTO)order;

        try
        {
            await conn.ExecuteUpdateAsync(orderDto, transaction);
            
            transaction.Commit();
        }
        catch (Exception e)
        {
            _logger.LogError("Update order failed, Exception: {Message}", e.Message);
            transaction.Rollback();
            throw;
        }
    }

    private async Task<IEnumerable<Order>> GetOrderImp(string sql)
    {
        await using var conn = new SqlConnection(_connectionString);

        var orderDictionary = new Dictionary<Guid, Order>();

        await conn.QueryAsync<OrderSqlDTO, FoodItemSqlDTO, Order>(sql,
            (orderSqlDto, foodItemDto) =>
            {
                var newFoodItemDto = (FoodItem?)foodItemDto;

                if (!orderDictionary.TryGetValue(orderSqlDto.Id, out var existingOrder))
                {
                    var newOrder = (Order)orderSqlDto;

                    if (newFoodItemDto != null)
                        newOrder.FoodItems.Add(newFoodItemDto);
                    
                    orderDictionary.Add(orderSqlDto.Id, newOrder);
                    return newOrder;
                }

                if (newFoodItemDto != null)
                    existingOrder.FoodItems.Add(newFoodItemDto);

                return existingOrder;
            }, splitOn: "OrderId");

        return orderDictionary.Values.ToList();
    }
}