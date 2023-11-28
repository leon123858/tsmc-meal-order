﻿using order.Attributes;
using order.Model;

namespace order.DTO.Sql;

public class OrderSqlDTO
{
    [UpdateKey] public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid RestaurantId { get; set; }
    public int Status { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime CreateTime { get; set; }

    public static implicit operator Order(OrderSqlDTO sqlDto)
    {
        return new Order
        {
            Id = sqlDto.Id,
            Status = (OrderStatus)sqlDto.Status,
            OrderDate = sqlDto.OrderDate,
            CreateTime = sqlDto.CreateTime
        };
    }

    public static explicit operator OrderSqlDTO(Order order)
    {
        return new OrderSqlDTO
        {
            Id = order.Id,
            UserId = order.Customer.Id,
            RestaurantId = order.Restaurant.Id,
            Status = (int)order.Status,
            OrderDate = order.OrderDate,
            CreateTime = order.CreateTime
        };
    }
}