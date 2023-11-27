using System.Collections;
using System.Data;
using Dapper;

namespace order.Repository.SqlImplement;

public static class DapperExtension
{
    public static async Task<int> QueryNewId<T>(this IDbConnection connection)
    {
        var className = typeof(T).Name;

        var sql = $"Select Isnull(Max(ID),0) + 1 from dbo.{className};";
        var response = await connection.QuerySingleOrDefaultAsync<int>(sql);

        return response;
    }

    public static async Task<int> GetLastestID<T>(this IDbConnection source)
    {
        var className = typeof(T).Name;

        if (!className.EndsWith("DTO")) throw new ArgumentException("class is not DTO.");
        var tableName = className[..^3];
        string sql = @$"Select Isnull(Max(ID),0) + 1 from {tableName};";

        var response = await source.QuerySingleOrDefaultAsync<int>(sql);
        return response;
    }

    public static async Task<IEnumerable<T>> QueryAllSetAsync<T>(this IDbConnection source, string condition = null)
    {
        var className = typeof(T).Name;

        if (!className.EndsWith("DTO")) throw new ArgumentException("class is not DTO.");

        var tableName = className[..^3];

        var sql = $"Select * from {tableName} {condition};";

        var response = await source.QueryAsync<T>(sql);
        return response;
    }

    public static async Task<int> ExecuteUpdateAsync(this IDbConnection connection, object obj,
        dynamic? conditionParam = null)
    {
        if (obj == null) throw new ArgumentNullException(nameof(obj));

        var sql = string.Empty;

        if (obj is IEnumerable enumerable and not (string or IEnumerable<KeyValuePair<string, object>>))
            foreach (var o in enumerable)
            {
                sql = SqlUtils.GetUpdateSql(o);
                break;
            }
        else
            sql = SqlUtils.GetUpdateSql(obj);

        if (string.IsNullOrWhiteSpace(sql)) return 0;

        return await connection.ExecuteAsync(sql, obj);
    }

    public static async Task<int> ExecuteInsertAsync(this IDbConnection connection, object obj)
    {
        if (obj == null) throw new ArgumentNullException(nameof(obj));

        var sql = string.Empty;

        if (obj is IEnumerable enumerable and not (string or IEnumerable<KeyValuePair<string, object>>))
            foreach (var o in enumerable)
            {
                sql = SqlUtils.GetInsertSql(o);
                break;
            }
        else
            sql = SqlUtils.GetInsertSql(obj);

        if (string.IsNullOrWhiteSpace(sql)) return 0;

        return await connection.ExecuteAsync(sql, obj);
    }

    public static async Task<int> ExecuteDeleteAsync(this IDbConnection connection, object obj)
    {
        if (obj == null) throw new ArgumentNullException(nameof(obj));

        var sql = string.Empty;

        if (obj is IEnumerable enumerable and not (string or IEnumerable<KeyValuePair<string, object>>))
            foreach (var o in enumerable)
            {
                sql = SqlUtils.GetDeleteSql(o);
                break;
            }
        else
            sql = SqlUtils.GetDeleteSql(obj);

        if (string.IsNullOrWhiteSpace(sql)) return 0;

        return await connection.ExecuteAsync(sql, obj);
    }
}