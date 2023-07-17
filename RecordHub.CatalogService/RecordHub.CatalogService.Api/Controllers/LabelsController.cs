using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecordHub.CatalogService.Application.Services;
using RecordHub.CatalogService.Domain.Models;

namespace RecordHub.CatalogService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LabelsController : ControllerBase
    {
        private readonly ILabelCatalogService _labelService;

        public LabelsController(ILabelCatalogService labelService)
        {
            _labelService = labelService;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateAsync(
            [FromBody] LabelModel model,
            CancellationToken cancellationToken)
        {
            await _labelService.AddAsync(model, cancellationToken);

            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id:Guid}")]
        public async Task<IActionResult> UpdateAsync(
            [FromRoute] Guid id,
            [FromBody] LabelModel model,
            CancellationToken cancellationToken = default)
        {
            await _labelService.UpdateAsync(id, model, cancellationToken);

            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:Guid}")]
        public async Task<IActionResult> DeleteAsync(
            [FromRoute] Guid id,
            CancellationToken cancellationToken = default)
        {
            await _labelService.DeleteAsync(id, cancellationToken);

            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
        {
            var countries = await _labelService.GetAllAsync(cancellationToken);

            return Ok(countries);
        }
    }
}
