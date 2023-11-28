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

    public SqlOrderRepository(IOptions<DatabaseConfig> config)
    {
        var secretPassword = Environment.GetEnvironmentVariable("SQL_PASSWORD") ?? config.Value.Password;
        _connectionString =
            $"Host={config.Value.Host};Username={config.Value.UserName};Password={secretPassword};Database={config.Value.DatabaseName}";
    }

    public async Task<IEnumerable<Order>> GetOrders(Guid userId)
    {
        var sql =
            @$"SELECT o.*, f.* FROM [order] AS o LEFT JOIN foodItem AS f on o.Id = f.OrderId WHERE o.UserId = '{userId}'";
        return await GetOrderImp(sql);
    }

    public async Task<Order> GetOrder(Guid userId, Guid orderId)
    {
        var sql =
            @$"SELECT o.*, f.* FROM [order] AS o LEFT JOIN foodItem AS f on o.Id = f.OrderId WHERE o.UserId = '{userId}' AND o.Id = '{orderId}'";
        return (await GetOrderImp(sql)).First();
    }

    public async Task CreateOrder(Order order)
    {
        await using var conn = new SqlConnection(_connectionString);
        var transaction = conn.BeginTransaction();

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
            await conn.ExecuteInsertAsync(orderDto);
            await conn.ExecuteInsertAsync(foodItemDtos);
            
            transaction.Commit();
        }
        catch (Exception e)
        {
            transaction.Rollback();
        }
    }

    public async Task UpdateOrder(Order order)
    {
        await using var conn = new SqlConnection(_connectionString);
        var transaction = conn.BeginTransaction();

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
            await conn.ExecuteUpdateAsync(orderDto, transaction);
            await conn.ExecuteUpdateAsync(foodItemDtos, transaction);
            
            transaction.Commit();
        }
        catch (Exception e)
        {
            transaction.Rollback();
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

                    orderDictionary.Add(orderSqlDto.Id, newOrder);
                    return newOrder;
                }

                if (newFoodItemDto != null)
                    existingOrder.FoodItems.Add(newFoodItemDto);

                return existingOrder;
            }, splitOn: "Id");

        return orderDictionary.Values.ToList();
    }
}