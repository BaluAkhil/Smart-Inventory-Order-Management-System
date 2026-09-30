using System.ComponentModel.DataAnnotations;

namespace OrderManagementSystem.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Sku { get; set; } = string.Empty;
        public string? Category { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; } = null!;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
