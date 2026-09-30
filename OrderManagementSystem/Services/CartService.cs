 using Microsoft.EntityFrameworkCore;
using OrderManagementSystem.Data;
using OrderManagementSystem.DTOs;
using OrderManagementSystem.Exceptions;
using OrderManagementSystem.Models;

namespace OrderManagementSystem.Services
{
    public interface ICartService
    {
        Task<CartDtos> GetCartAsync(int userId);
        Task<CartDtos> AddItemAsync(int userId, AddCartItemDto dto);
        Task<CartDtos> UpdateItemAsync(int userId, int cartItemId, UpdateCartItemDto dto);
        Task<CartDtos> RemoveItemAsync(int userId, int cartItemId);
    }

    public class CartService : ICartService
    {
        private readonly AppDbContext _context;

        public CartService(AppDbContext context) => _context = context;

        public async Task<CartDtos> GetCartAsync(int userId)
        {
            var cart = await GetCart(userId);
            return ToDto(cart);
        }

        public async Task<CartDtos> AddItemAsync(int userId, AddCartItemDto dto)
        {
            var cart = await GetCart(userId);

            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == dto.ProductId);

            if (product == null)
                throw new NotFoundException("Product not found.");

            if (!product.IsActive)
                throw new BadRequestException("Product is not available.");

            var item = cart.Items.FirstOrDefault(i => i.ProductId == dto.ProductId);

            int quantity = (item?.Quantity ?? 0) + dto.Quantity;

            if (quantity > product.Quantity)
                throw new BadRequestException("Insufficient stock.");

            if (item != null)
            {
                item.Quantity = quantity;
            }
            else
            {
                cart.Items.Add(new CartItem
                {
                    CartId = cart.CartId,
                    ProductId = product.ProductId,
                    Quantity = dto.Quantity
                });
            }

            await _context.SaveChangesAsync();

            return ToDto(cart);
        }

        public async Task<CartDtos> UpdateItemAsync(int userId, int cartItemId, UpdateCartItemDto dto)
        {
            var cart = await GetCart(userId);

            var item = cart.Items.FirstOrDefault(i => i.CartItemId == cartItemId);

            if (item == null)
                throw new NotFoundException("Cart item not found.");

            if (dto.Quantity > item.Product.Quantity)
                throw new BadRequestException("Insufficient stock.");

            item.Quantity = dto.Quantity;

            await _context.SaveChangesAsync();

            return ToDto(cart);
        }

        public async Task<CartDtos> RemoveItemAsync(int userId, int cartItemId)
        {
            var cart = await GetCart(userId);

            var item = cart.Items.FirstOrDefault(i => i.CartItemId == cartItemId);

            if (item == null)
                throw new NotFoundException("Cart item not found.");

            _context.CartItems.Remove(item);

            await _context.SaveChangesAsync();

            return ToDto(cart);
        }

        private async Task<Cart> GetCart(int userId)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart != null)
                return cart;

            cart = new Cart
            {
                UserId = userId
            };

            _context.Carts.Add(cart);

            await _context.SaveChangesAsync();

            return cart;
        }

        private CartDtos ToDto(Cart cart)
        {
            return new CartDtos
            {
                CartId = cart.CartId,
                Items = cart.Items.Select(i => new CartItemDto
                {
                    CartItemId = i.CartItemId,
                    ProductId = i.ProductId,
                    ProductName = i.Product.ProductName,
                    UnitPrice = i.Product.Price,
                    Quantity = i.Quantity,
                    AvailableStock = i.Product.Quantity
                }).ToList()
            };
        }
    }
}
