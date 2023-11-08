namespace order.Model
{
    public class Order
    {
        public OrderStatus Status { get; set; }
        public Guid Id { get; set; }

        public void Confirm()
        {
            Status = OrderStatus.Preparing;
        }
    }
}
