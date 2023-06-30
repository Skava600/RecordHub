using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecordHub.OrderingService.Application.Services;
using RecordHub.OrderingService.Domain.Models;
using System.Security.Claims;

namespace RecordHub.OrderingService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderingService orderingService;

        public OrdersController(IOrderingService orderingService)
        {
            this.orderingService = orderingService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PostOrder([FromBody] OrderModel model, CancellationToken cancellationToken = default)
        {
            string? userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await orderingService.AddOrderAsync(userId, model, cancellationToken);

            return Ok();
        }
    }
}
