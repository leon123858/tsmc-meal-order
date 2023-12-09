using System.ComponentModel.DataAnnotations.Schema;
using core.Model;
using order.Attributes;
using order.Model;

namespace order.DTO.Sql;

[Table("foodItem")]
public class FoodItemSqlDTO
{
    public Guid OrderId { get; set; }
    public string? Snapshot_Description { get; set; }
    public string Snapshot_Name { get; set; } = "";
    public int Snapshot_Price { get; set; }
    public int Snapshot_Count { get; set; }
    public string? Snapshot_ImageUrl { get; set; }
    public string Snapshot_Tags { get; set; } = "";
    public int Count { get; set; }
    public string Description { get; set; } = "";

    public static implicit operator OrderedFoodItem(FoodItemSqlDTO sqlDto)
    {
        var foodItem = new FoodItem
        {
            Description = sqlDto.Snapshot_Description,
            Name = sqlDto.Snapshot_Name,
            Price = sqlDto.Snapshot_Price,
            Count = sqlDto.Snapshot_Count,
            ImageUrl = sqlDto.Snapshot_ImageUrl,
            Tags = sqlDto.Snapshot_Tags.Split(",").ToList()
        };

        return new OrderedFoodItem(foodItem, sqlDto.Count, sqlDto.Description);
    }

    public static explicit operator FoodItemSqlDTO(OrderedFoodItem foodItem)
    {
        return new FoodItemSqlDTO
        {
            Snapshot_Description = foodItem.Snapshot.Description,
            Snapshot_Name = foodItem.Snapshot.Name,
            Snapshot_Price = foodItem.Snapshot.Price,
            Snapshot_Count = foodItem.Snapshot.Count,
            Snapshot_ImageUrl = foodItem.Snapshot.ImageUrl,
            Snapshot_Tags = string.Join(",", foodItem.Snapshot.Tags),
            Count = foodItem.Count,
            Description = foodItem.Description
        };
    }
}