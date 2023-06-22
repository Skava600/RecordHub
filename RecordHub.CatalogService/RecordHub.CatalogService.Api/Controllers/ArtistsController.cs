using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecordHub.CatalogService.Application.Services;
using RecordHub.CatalogService.Domain.Models;

namespace RecordHub.CatalogService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArtistsController : ControllerBase
    {
        private readonly IArtistCatalogService _artistService;

        public ArtistsController(IArtistCatalogService artistService)
        {
            _artistService = artistService;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] ArtistModel model, CancellationToken cancellationToken)
        {
            await _artistService.AddAsync(model, cancellationToken);

            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id:Guid}")]
        public async Task<IActionResult> UpdateAsync([FromRoute] Guid id,
           [FromBody] ArtistModel model,
           CancellationToken cancellationToken = default)
        {
            await _artistService.UpdateAsync(id, model, cancellationToken);

            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:Guid}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid id,
           CancellationToken cancellationToken = default)
        {
            await _artistService.DeleteAsync(id, cancellationToken);

            return NoContent();
        }

        [HttpGet("{slug}")]
        public async Task<IActionResult> GetBySlug([FromRoute] string slug, CancellationToken cancellationToken)
        {
            var artist = await _artistService.GetBySlug(slug, cancellationToken);

            return Ok(artist);
        }
    }
}
