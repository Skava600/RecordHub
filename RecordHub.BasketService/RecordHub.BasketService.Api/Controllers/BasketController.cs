using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecordHub.BasketService.Application.Services;
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
        public async Task<IActionResult> GetBasketAsync(CancellationToken cancellationToken = default)
        {
            string? userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return Ok(await basketService.GetBasketAsync(userId, cancellationToken));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UpdateBasketAsync(
            [FromBody] BasketItemModel cartItem,
            CancellationToken cancellationToken = default)
        {
            string? userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var basket = await basketService.UpdateBasketItemAsync(userId, cartItem, cancellationToken);

            return Ok(basket);
        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> ClearBasketAsync(
            CancellationToken cancellationToken = default)
        {
            string? userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await basketService.ClearBasketAsync(userId, cancellationToken);

            return Ok();
        }

        [HttpDelete("{productId}")]
        [Authorize]
        public async Task<IActionResult> DeleteItemAsync(
            [FromRoute] string productId,
            CancellationToken cancellationToken = default)
        {
            string? userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var basket = await basketService.RemoveBasketItemAsync(userId, productId, cancellationToken);

            return Ok(basket);
        }

        [HttpPost("checkout")]
        [Authorize]
        public async Task<IActionResult> BasketCheckoutAsync(
            [FromBody] BasketCheckoutModel model,
            CancellationToken cancellationToken = default)
        {
            await basketService.CheckoutAsync(model, cancellationToken);

            return Accepted();
        }
    }
}
