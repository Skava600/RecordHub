using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecordHub.CatalogService.Application.Services;
using RecordHub.CatalogService.Domain.Models;

namespace RecordHub.CatalogService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StylesController : ControllerBase
    {
        private readonly IStyleCatalogService _styleService;

        public StylesController(IStyleCatalogService styleService)
        {
            _styleService = styleService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
        {
            var styles = await _styleService.GetAllAsync(cancellationToken);

            return Ok(styles);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateAsync(
            [FromBody] StyleModel model,
            CancellationToken cancellationToken)
        {
            await _styleService.AddAsync(model, cancellationToken);

            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id:Guid}")]
        public async Task<IActionResult> UpdateAsync(
            [FromRoute] Guid id,
            [FromBody] StyleModel model,
            CancellationToken cancellationToken = default)
        {
            await _styleService.UpdateAsync(id, model, cancellationToken);

            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:Guid}")]
        public async Task<IActionResult> DeleteAsync(
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            await _styleService.DeleteAsync(id, cancellationToken);

            return NoContent();
        }
    }
}
