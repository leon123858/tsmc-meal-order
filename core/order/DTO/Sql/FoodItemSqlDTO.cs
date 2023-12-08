using System.ComponentModel.DataAnnotations.Schema;
using core.Model;
using order.Attributes;

namespace order.DTO.Sql;

[Table("foodItem")]
public class FoodItemSqlDTO
{
    public Guid OrderId { get; set; }
    public string? Description { get; set; }
    public string Name { get; set; } = "";
    public int Price { get; set; }
    public int Count { get; set; }
    public string? ImageUrl { get; set; }
    public string Tags { get; set; } = "";

    public static implicit operator FoodItem(FoodItemSqlDTO sqlDto)
    {
        return new FoodItem
        {
            Description = sqlDto.Description,
            Name = sqlDto.Name,
            Price = sqlDto.Price,
            Count = sqlDto.Count,
            ImageUrl = sqlDto.ImageUrl,
            Tags = sqlDto.Tags.Split(",").ToList()
        };
    }

    public static explicit operator FoodItemSqlDTO(FoodItem foodItem)
    {
        return new FoodItemSqlDTO
        {
            Description = foodItem.Description,
            Name = foodItem.Name,
            Price = foodItem.Price,
            Count = foodItem.Count,
            ImageUrl = foodItem.ImageUrl,
            Tags = string.Join(",", foodItem.Tags)
        };
    }
}