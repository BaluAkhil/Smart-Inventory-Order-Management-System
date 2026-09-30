using System.ComponentModel.DataAnnotations;

namespace OrderManagementSystem.DTOs
{
    public class ProductDtos
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = "";
        public string? Description { get; set; }
        public string Sku { get; set; } = "";
        public string? Category { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

    }
    public class CreateProductDto
    {
        [Required, StringLength(200, MinimumLength =2)]
        public string ProductName { get; set; } = "";

        [StringLength(1000)]
        public string? Description { get; set; }

        [Required, StringLength(50)]
        public string Sku { get; set; } = "";

        [StringLength(100)]
        public string? Category { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }

    }

    public class UpdateProductDto
    {
        [Required, StringLength(150, MinimumLength = 2)]
        public string ProductName { get; set; } = "";

        [StringLength(1000)]
        public string? Description { get; set; }

        [StringLength(100)]
        public string? Category { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class ProductQueryParams
    {
        public string? Search { get; set; }
        public string? Category { get; set; }

        [Range(1, int.MaxValue)]
        public int Page { get; set; } = 1;

        [Range(1, 100)]
        public int PageSize { get; set; } = 10;
    }
}
