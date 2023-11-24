using order.Attributes;

namespace order.Repository;

public class RepositoryUtils
{
    public static string GetUpdateSql(object obj)
    {
        var type = obj.GetType();
        return GetUpdateSql(type);
    }

    public static string GetUpdateSql(Type type)
    {
        var className = type.Name;

        if (!className.EndsWith("DTO")) throw new ArgumentException("class is not DTO.");

        var tableName = className[..^3];

        var updateSql = $"UPDATE {tableName} SET ";
        var updateKey = new List<string>();

        var properties = type.GetProperties();

        foreach (var property in properties)
        {
            if (property.IsDefined(typeof(UpdateKeyAttribute), false))
            {
                updateKey.Add(property.Name);
                continue;
            }

            updateSql += $"{property.Name} = @{property.Name},";
        }

        if (!updateKey.Any()) throw new ArgumentException("class has no update key!");

        updateSql = updateSql.TrimEnd(',');
        updateSql += " WHERE 1 = 1 ";

        foreach (var key in updateKey)
            updateSql += $"AND {key} = @{key} ";

        updateSql += ";";

        return updateSql;
    }

    public static string GetUpdateSql<T>()
    {
        return GetUpdateSql(typeof(T));
    }

    public static string GetDeleteSql(object obj)
    {
        var type = obj.GetType();
        return GetDeleteSql(type);
    }

    public static string GetDeleteSql(Type type)
    {
        var className = type.Name;

        if (!className.EndsWith("DTO")) throw new ArgumentException("class is not DTO.");

        var tableName = className[..^3];

        var deleteSql = $"Delete {tableName} ";
        var updateKey = new List<string>();

        var properties = type.GetProperties();

        foreach (var property in properties)
        {
            if (!property.IsDefined(typeof(UpdateKeyAttribute), false)) continue;
            updateKey.Add(property.Name);
        }

        if (!updateKey.Any()) throw new ArgumentException("class has no update key!");

        deleteSql = deleteSql.TrimEnd(',');
        deleteSql += " WHERE 1 = 1 ";

        foreach (var key in updateKey)
            deleteSql += $"AND {key} = @{key} ";

        deleteSql += ";";

        return deleteSql;
    }

    public static string GetDeleteSql<T>()
    {
        return GetDeleteSql(typeof(T));
    }

    public static string GetInsertSql(Type type)
    {
        var className = type.Name;

        if (!className.EndsWith("DTO")) throw new ArgumentException("class is not DTO.");

        var tableName = className[..^3];

        var insertSql = $"INSERT INTO {tableName} (";

        var properties = type.GetProperties();

        foreach (var property in properties)
            insertSql += $"{property.Name},";

        insertSql = insertSql.TrimEnd(',');
        insertSql += ") VALUES (";

        foreach (var property in properties)
            insertSql += $"@{property.Name},";

        insertSql = insertSql.TrimEnd(',');
        insertSql += ");";

        return insertSql;
    }

    public static string GetInsertSql(object obj)
    {
        var type = obj.GetType();
        return GetInsertSql(type);
    }

    public static string GetInsertSql<T>()
    {
        return GetInsertSql(typeof(T));
    }
}