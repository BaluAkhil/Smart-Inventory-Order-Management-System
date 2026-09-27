using OrderManagementSystem.Models;
using System.ComponentModel.DataAnnotations;

namespace OrderManagementSystem.DTOs
{
    public class CartDtos
    {
        public int Id { get; set; }
        public List<CartItemDto> Item { get; set; } = new();

        public decimal TotalAmount => Item.Sum(a => a.Total);
    }

    public class CartItemDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public int AvailableStock { get; set; }
        public decimal Total => UnitPrice * Quantity;
    }

    public class AddCartItemDto
    {
        [Required]
        public int ProductId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; } = 1;
    }

    public class UpdateCartItemDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }
    }
}
