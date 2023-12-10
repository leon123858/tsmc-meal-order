using System.Collections;
using System.Data;
using Dapper;

namespace order.Repository.SqlImplement;

public static class DapperExtension
{
    public static async Task<IEnumerable<T>> QueryAllSetAsync<T>(this IDbConnection source, string condition = null)
    {
        var className = typeof(T).Name;

        if (!className.EndsWith("DTO")) throw new ArgumentException("class is not DTO.");

        var tableName = className[..^6].ToLower();

        var sql = $"Select * from {tableName} {condition};";

        var response = await source.QueryAsync<T>(sql);
        return response;
    }

    public static async Task<int> ExecuteUpdateAsync(this IDbConnection connection, object obj,
        IDbTransaction? transaction = null)
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

        return await connection.ExecuteAsync(sql, obj, transaction);
    }

    public static async Task<int> ExecuteInsertAsync(this IDbConnection connection, object obj,
        IDbTransaction? transaction = null)
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

        return await connection.ExecuteAsync(sql, obj, transaction);
    }
}