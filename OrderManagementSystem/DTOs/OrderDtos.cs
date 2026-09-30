using System.ComponentModel.DataAnnotations;
using OrderManagementSystem.Models;

namespace OrderManagementSystem.DTOs
{
    public class OrderDtos
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; }
        public decimal TotalAmount { get; set; }

        public List<OrderItemDto> Items { get; set; } = new();
    }

    public class OrderItemDto
    {
        public int OrderItemId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = "";
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total { get; set; }
    }

    public class UpdateOrderStatusDto
    {
        [Required]
        public OrderStatus Status { get; set; }
    }
}