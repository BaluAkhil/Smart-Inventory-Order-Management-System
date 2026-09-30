using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderManagementSystem.DTOs;
using OrderManagementSystem.Models;
using OrderManagementSystem.Services;

namespace OrderManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<PagedResult<ProductDtos>>> GetAll([FromQuery] ProductQueryParams query)
        {
            var result = await _productService.GetProductsAsync(query, false);

            return Ok(result);
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<ProductDtos>> GetById(int id)
        {
            var result = await _productService.GetByIdAsync(id, false);

            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<ActionResult<ProductDtos>> Create(CreateProductDto dto)
        {
            var result = await _productService.CreateAsync(dto);

            return CreatedAtAction(nameof(GetById), new { id = result.ProductId }, result);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<ActionResult<ProductDtos>> Update(int id,UpdateProductDto dto)
        {
            var result = await _productService.UpdateAsync(id, dto);

            return Ok(result);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> Delete(int id)
        {
            await _productService.DeleteAsync(id);

            return NoContent();
        }
    }
}
