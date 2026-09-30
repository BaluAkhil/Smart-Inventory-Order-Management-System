using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderManagementSystem.DTOs;
using OrderManagementSystem.Services;
using System.Security.Claims;

namespace OrderManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet]
        public async Task<ActionResult<CartDtos>> GetMyCart()
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var result = await _cartService.GetCartAsync(userId);

            return Ok(result);
        }


        [HttpPost("items")]
        public async Task<ActionResult<CartDtos>> AddItems(AddCartItemDto dto)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var result = await _cartService.AddItemAsync(userId, dto);

            return Ok(result);
        }


        [HttpPut("itesm/{cartItemId}")]
        public async Task<ActionResult<CartDtos>> UpdateItem(int cartItemId, UpdateCartItemDto dto)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var result = await _cartService.UpdateItemAsync(userId, cartItemId, dto);

            return Ok(result);
        }


        [HttpDelete("items/{cartItemId}")]
        public async Task<ActionResult<CartDtos>> RemoveItem(int cartItemId)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var result = await _cartService.RemoveItemAsync(
                userId, cartItemId);

            return Ok(result);
        }

    }
}