using order.Attributes;
using order.Model;

namespace order.DTO.Sql;

public class OrderSqlDTO
{
    [UpdateKey] public int Id { get; set; }

    public OrderStatus Status { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime CreateTime { get; set; }
}