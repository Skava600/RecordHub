using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecordHub.CatalogService.Application.Services;
using RecordHub.CatalogService.Domain.Models;

namespace RecordHub.CatalogService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecordController : ControllerBase
    {
        private readonly IRecordCatalogService recordCatalogService;

        public RecordController(IRecordCatalogService recordCatalogService)
        {
            this.recordCatalogService = recordCatalogService;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateAsync(
            [FromBody] RecordModel model,
            CancellationToken cancellationToken = default)
        {
            await recordCatalogService.AddAsync(model, cancellationToken);

            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id:Guid}")]
        public async Task<IActionResult> UpdateAsync(
            [FromRoute] Guid id,
            [FromBody] RecordModel model,
            CancellationToken cancellationToken = default)
        {
            await recordCatalogService.UpdateAsync(id, model, cancellationToken);

            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:Guid}")]
        public async Task<IActionResult> DeleteAsync(
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            await recordCatalogService.DeleteAsync(id, cancellationToken);

            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> GetByPageAsync(
            [FromQuery] int page,
            [FromQuery] int pageSize,
            CancellationToken cancellationToken = default)
        {
            return Ok(await recordCatalogService.GetByPageAsync(page, pageSize, cancellationToken));
        }

        [HttpGet("filter")]
        public async Task<IActionResult> SearchAsync(
            [FromQuery] RecordFilterModel model,
            CancellationToken cancellationToken = default)
        {
            return Ok(await recordCatalogService.GetByPageAsync(model, cancellationToken));
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchAsync(
            [FromQuery] string text,
            CancellationToken cancellationToken = default)
        {
            return Ok(await recordCatalogService.SearchAsync(text, cancellationToken));
        }
    }
}
