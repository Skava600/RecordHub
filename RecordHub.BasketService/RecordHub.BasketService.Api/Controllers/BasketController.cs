using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecordHub.BasketService.Applicatation.Services;
using RecordHub.BasketService.Domain.Models;
using System.Security.Claims;

namespace RecordHub.BasketService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        private readonly IBasketService basketService;

        public BasketController(IBasketService basketService)
        {
            this.basketService = basketService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetBasketAsync()
        {
            string? userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Ok(await basketService.GetBasketAsync(userId));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UpdateBasketAsync([FromBody] ShoppingCartItemModel cartItem)
        {
            string? userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await basketService.UpdateCartItemAsync(userId, cartItem);

            return Ok();
        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> ClearBasketAsync()
        {
            string? userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await basketService.ClearBasketAsync(userId);

            return Ok();
        }
    }
}
