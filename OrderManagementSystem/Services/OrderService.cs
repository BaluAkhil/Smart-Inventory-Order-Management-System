using Microsoft.EntityFrameworkCore;
using OrderManagementSystem.Data;
using OrderManagementSystem.DTOs;
using OrderManagementSystem.Exceptions;
using OrderManagementSystem.Models;

namespace OrderManagementSystem.Services
{
    public interface IOrderService
    {
        Task<OrderDtos> PlaceOrderAsync(int userId);
        Task<PagedResult<OrderDtos>> GetMyOrdersAsync(int userId, int page, int pageSize);
        Task<OrderDtos> GetOrderByIdAsync(int userId, int orderId, bool isAdmin);
        Task<PagedResult<OrderDtos>> GetAllOrdersAsync(int page, int pageSize, OrderStatus? status);
        Task<OrderDtos> UpdateOrderStatusAsync(int orderId, OrderStatus status);
    }

    public class OrderService : IOrderService
    {
        private readonly AppDbContext _context;

        public OrderService(AppDbContext context)
        {
            _context = context;
        }

        // Place Order
        public async Task<OrderDtos> PlaceOrderAsync(int userId)
        {
            const int maxAttempts = 3;

            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                try
                {
                    return await PlaceOrderAttemptAsync(userId);
                }
                catch (DbUpdateConcurrencyException) when (attempt < maxAttempts)
                {
                    foreach (var entry in _context.ChangeTracker.Entries().ToList())
                    {
                        entry.State = EntityState.Detached;
                    }
                }
            }

            throw new ConflictException(
                "Could not place the order because stock changed while processing it. Please try again.");
        }

        // Place Order Attempt
        private async Task<OrderDtos> PlaceOrderAttemptAsync(int userId)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || cart.Items.Count == 0)
            {
                throw new BadRequestException("Cart is empty.");
            }

            using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                var order = new Order
                {
                    UserId = userId,
                    Status = OrderStatus.Pending,
                    OrderDate = DateTime.UtcNow
                };

                decimal total = 0;

                foreach (var item in cart.Items)
                {
                    var product = item.Product;

                    if (!product.IsActive)
                    {
                        throw new BadRequestException(
                            $"Product '{product.Name}' is not available.");
                    }

                    if (product.Quantity < item.Quantity)
                    {
                        throw new BadRequestException(
                            $"Insufficient stock for '{product.Name}'.");
                    }

                    product.Quantity -= item.Quantity;

                    var orderItem = new OrderItem
                    {
                        ProductId = product.Id,
                        ProductName = product.Name,
                        Quantity = item.Quantity,
                        UnitPrice = product.Price
                    };

                    order.Items.Add(orderItem);

                    total += product.Price * item.Quantity;
                }

                order.TotalAmount = total;

                _context.Orders.Add(order);

                _context.CartItems.RemoveRange(cart.Items);

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return ToDto(order);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // Get Customer Orders
        public async Task<PagedResult<OrderDtos>> GetMyOrdersAsync(
            int userId,
            int page,
            int pageSize)
        {
            page = page < 1 ? 1 : page;

            pageSize = pageSize < 1 || pageSize > 100
                ? 10
                : pageSize;

            var query = _context.Orders
                .AsNoTracking()
                .Where(o => o.UserId == userId);

            return await GetOrders(query, page, pageSize);
        }

        // Get Order By ID
        public async Task<OrderDtos> GetOrderByIdAsync(
            int userId,
            int orderId,
            bool isAdmin)
        {
            var order = await _context.Orders
                .AsNoTracking()
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                throw new NotFoundException("Order not found.");
            }

            if (!isAdmin && order.UserId != userId)
            {
                throw new NotFoundException("Order not found.");
            }

            return ToDto(order);
        }

        // Admin - Get All Orders
        public async Task<PagedResult<OrderDtos>> GetAllOrdersAsync(
            int page,
            int pageSize,
            OrderStatus? status)
        {
            page = page < 1 ? 1 : page;

            pageSize = pageSize < 1 || pageSize > 100
                ? 10
                : pageSize;

            var query = _context.Orders
                .AsNoTracking()
                .AsQueryable();

            if (status.HasValue)
            {
                query = query.Where(o => o.Status == status.Value);
            }

            return await GetOrders(query, page, pageSize);
        }

        // Update Order Status
        public async Task<OrderDtos> UpdateOrderStatusAsync(
            int orderId,
            OrderStatus status)
        {
            var order = await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                throw new NotFoundException("Order not found.");
            }

            if (order.Status == OrderStatus.Delivered ||
                order.Status == OrderStatus.Cancelled)
            {
                throw new BadRequestException(
                    $"Order is already {order.Status} and cannot be updated.");
            }

            order.Status = status;

            await _context.SaveChangesAsync();

            return ToDto(order);
        }

        // Pagination
        private async Task<PagedResult<OrderDtos>> GetOrders(
            IQueryable<Order> query,
            int page,
            int pageSize)
        {
            int totalCount = await query.CountAsync();

            var orders = await query
                .Include(o => o.Items)
                .OrderByDescending(o => o.OrderDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<OrderDtos>
            {
                Items = orders.Select(ToDto).ToList(),
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        // Convert Order to DTO
        private OrderDtos ToDto(Order order)
        {
            return new OrderDtos
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                Status = order.Status,
                TotalAmount = order.TotalAmount,

                Items = order.Items.Select(i => new OrderItemDto
                {
                    ProductId = i.ProductId,
                    ProductName = i.ProductName,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    Total = i.Quantity * i.UnitPrice
                }).ToList()
            };
        }
    }
}