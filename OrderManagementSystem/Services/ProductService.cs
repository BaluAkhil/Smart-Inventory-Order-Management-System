using Microsoft.EntityFrameworkCore;
using OrderManagementSystem.Data;
using OrderManagementSystem.DTOs;
using OrderManagementSystem.Exceptions;
using OrderManagementSystem.Models;

namespace OrderManagementSystem.Services
{
    public interface IProductService
    {
        Task<PagedResult<ProductDtos>> GetProductsAsync(ProductQueryParams query, bool includeInactive);

        Task<ProductDtos> GetByIdAsync(int id, bool includeInactive);

        Task<ProductDtos> CreateAsync(CreateProductDto dto);

        Task<ProductDtos> UpdateAsync(int id, UpdateProductDto dto);

        Task DeleteAsync(int id);
    }

    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;

        public ProductService(AppDbContext context) => _context = context;

        public async Task<PagedResult<ProductDtos>> GetProductsAsync(ProductQueryParams query, bool includeInactive)
        {
            var products = _context.Products.AsNoTracking();

            if (!includeInactive)
                products = products.Where(p => p.IsActive);

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                string search = query.Search.Trim().ToLower();

                products = products.Where(p =>
                    p.ProductName.ToLower().Contains(search) ||
                    p.Sku.ToLower().Contains(search) ||
                    (p.Category != null && p.Category.ToLower().Contains(search)));
            }

            if (!string.IsNullOrWhiteSpace(query.Category))
            {
                string category = query.Category.Trim().ToLower();

                products = products.Where(p =>
                    p.Category != null &&
                    p.Category.ToLower() == category);
            }

            int totalCount = await products.CountAsync();

            int page = query.Page < 1 ? 1 : query.Page;

            int pageSize = query.PageSize < 1 || query.PageSize > 100
                ? 10
                : query.PageSize;

            var productList = await products
                .OrderBy(p => p.ProductName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<ProductDtos>
            {
                Items = productList.Select(ToDto).ToList(),
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<ProductDtos> GetByIdAsync(int id, bool includeInactive)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null ||(!includeInactive && !product.IsActive))
            {
                throw new NotFoundException("Product not found.");
            }

            return ToDto(product);
        }

        public async Task<ProductDtos> CreateAsync(CreateProductDto dto)
        {
            string sku = dto.Sku.Trim().ToUpperInvariant();

            if (await _context.Products.AnyAsync(p => p.Sku == sku))
                throw new ConflictException("SKU already exists.");

            var product = new Product
            {
                ProductName = dto.ProductName,
                Description = dto.Description?.Trim(),
                Sku = sku,
                Category = dto.Category?.Trim(),
                Price = dto.Price,
                Quantity = dto.Quantity,
                IsActive = true
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return ToDto(product);
        }

        public async Task<ProductDtos> UpdateAsync(int id, UpdateProductDto dto)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
                throw new NotFoundException("Product not found.");

            product.ProductName = dto.ProductName.Trim();
            product.Description = dto.Description?.Trim();
            product.Category = dto.Category?.Trim();
            product.Price = dto.Price;
            product.Quantity = dto.Quantity;
            product.IsActive = dto.IsActive;
            product.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return ToDto(product);
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
                throw new NotFoundException("Product not found.");

            product.IsActive = false;
            product.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        private ProductDtos ToDto(Product product)
        {
            return new ProductDtos
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                Description = product.Description,
                Sku = product.Sku,
                Category = product.Category,
                Price = product.Price,
                Quantity = product.Quantity,
                IsActive = product.IsActive,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt
            };
        }
    }
}