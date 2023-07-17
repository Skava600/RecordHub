using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecordHub.IdentityService.Core.Services;
using RecordHub.IdentityService.Domain.Models;
using System.Security.Claims;

namespace RecordHub.IdentityService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressesController : ControllerBase
    {
        private readonly IAddressService _addressService;

        public AddressesController(IAddressService addressService)
        {
            _addressService = addressService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddAddressAsync(
            [FromBody] AddressModel model,
            CancellationToken cancellationToken = default)
        {
            string? userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await _addressService.AddAsync(userId, model, cancellationToken);

            return Ok();
        }

        [HttpPut("{id:Guid}")]
        [Authorize]
        public async Task<IActionResult> UpdateAddressAsync(
            Guid id,
            [FromBody] AddressModel model,
            CancellationToken cancellationToken = default)
        {
            await _addressService.UpdateAsync(id, model, cancellationToken);

            return Ok();
        }

        [HttpDelete("{id:Guid}")]
        [Authorize]
        public async Task<IActionResult> DeleteAddressAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            await _addressService.DeleteAsync(id, cancellationToken);

            return Ok();
        }
    }
}
