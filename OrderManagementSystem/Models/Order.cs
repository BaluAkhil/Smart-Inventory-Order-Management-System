namespace OrderManagementSystem.Models
{
    public class Order
    {
        public int OrderId { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public decimal TotalAmount { get; set; }

        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}
