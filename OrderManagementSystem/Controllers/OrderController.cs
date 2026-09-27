using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderManagementSystem.DTOs;
using OrderManagementSystem.Models;
using OrderManagementSystem.Services;
using System.Security.Claims;

namespace OrderManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        public async Task<ActionResult<OrderDtos>> PlaceOrder()
        {
            var result = await _orderService.PlaceOrderAsync(GetUserId());
            return StatusCode(201, result);
        }

        [HttpGet]
        public async Task<ActionResult> GetMyOrders(int page = 1, int pageSize = 10)
        {
            var result = await _orderService.GetMyOrdersAsync(GetUserId(), page, pageSize);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<OrderDtos>> GetById(int id)
        {
            var result = await _orderService.GetOrderByIdAsync(GetUserId(), id, User.IsInRole(UserRoles.Admin));
            return Ok(result);
        }

        [HttpGet("admin/all")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<ActionResult> GetAll(int page = 1, int pageSize = 10, OrderStatus? status = null)
        {
            var result = await _orderService.GetAllOrdersAsync(page, pageSize, status);
            return Ok(result);
        }

        [HttpPut("{id:int}/status")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<ActionResult<OrderDtos>> UpdateStatus(int id, UpdateOrderStatusDto dto)
        {
            var result = await _orderService.UpdateOrderStatusAsync(id, dto.Status);
            return Ok(result);
        }

        private int GetUserId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }
    }
}