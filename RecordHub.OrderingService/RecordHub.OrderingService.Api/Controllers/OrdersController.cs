using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecordHub.OrderingService.Application.Services;
using RecordHub.Shared.Enums;

namespace RecordHub.OrderingService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderingService _orderingService;

        public OrdersController(IOrderingService orderingService)
        {
            _orderingService = orderingService;
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{orderId}/{state}")]
        public async Task<IActionResult> ChangeOrderState(
            [FromRoute] Guid orderId,
            [FromRoute] StatesEnum state)
        {
            await _orderingService.ChangeOrderStateAsync(orderId, state);

            return Ok();
        }

        [Authorize]
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUsersOrders(
            string userId,
            CancellationToken cancellationToken = default)
        {
            var user = HttpContext.User;
            var orders = await _orderingService.GetUsersOrdersAsync(userId, user, cancellationToken);

            return Ok(orders);
        }

    }
}
