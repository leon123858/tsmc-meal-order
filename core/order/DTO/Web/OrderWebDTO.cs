using core.Model;
using order.Model;

namespace order.DTO.Web;

public class OrderWebDTO
{
    public string Status { get; set; }
    public Guid Id { get; set; }
    public User Customer { get; set; }
    public User Restaurant { get; set; }
    public List<OrderedFoodItem> FoodItems { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime CreateTime { get; set; }
    public string MealType { get; set; }

    public static explicit operator OrderWebDTO(Order order)
    {
        return new OrderWebDTO
        {
            Status = order.Status.ToString(),
            Id = order.Id,
            Customer = order.Customer,
            Restaurant = order.Restaurant,
            FoodItems = order.FoodItems,
            OrderDate = order.OrderDate,
            CreateTime = order.CreateTime,
            MealType = order.MealType.ToString()
        };
    }
}