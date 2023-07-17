using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecordHub.IdentityService.Core.Services;
using RecordHub.IdentityService.Domain.Models;
using System.Security.Claims;

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
               model, cancellationToken);

            return Ok(jwtToken);
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync(
            [FromBody] RegisterModel model,
            CancellationToken cancellationToken = default)
        {
            await accountService.RegisterAsync(
                model, cancellationToken);

            return Ok();
        }

        [HttpGet("info")]
        [Authorize]
        public async Task<IActionResult> UserInfo(
            CancellationToken cancellationToken = default)
        {
            string? userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await accountService.GetUserInfoAsync(userId, cancellationToken);

            return Ok(user);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> VerifyEmailAsync(
            [FromBody] string token,
            CancellationToken cancellationToken = default)
        {
            string? userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await accountService.VerifyEmailAsync(token, userId, cancellationToken);

            return Ok();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> SendVerificationEmail(
            CancellationToken cancellationToken = default)
        {
            string? userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            await accountService.SendEmailVerificationAsync(userId, cancellationToken);

            return Ok();
        }
    }
}
