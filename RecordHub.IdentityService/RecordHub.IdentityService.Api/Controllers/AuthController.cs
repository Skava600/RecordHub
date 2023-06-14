using Microsoft.AspNetCore.Mvc;
using RecordHub.IdentityService.Core.Services;
using RecordHub.IdentityService.Domain.Models;

namespace RecordHub.IdentityService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAccountService accountService;

        public AuthController(IAccountService accountService)
        {
            this.accountService = accountService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync(
            [FromBody] LoginModel model,
            CancellationToken cancellationToken = default)
        {
            var jwtToken = await accountService.LoginAsync(
               model, HttpContext, cancellationToken);

            return Ok(jwtToken);
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync(
            [FromBody] RegisterModel model,
            CancellationToken cancellationToken = default)
        {
            await accountService.RegisterAsync(
                model, HttpContext, cancellationToken);

            return Ok();
        }


    }
}
