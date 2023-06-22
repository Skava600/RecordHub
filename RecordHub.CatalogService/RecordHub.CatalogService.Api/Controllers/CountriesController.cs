using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecordHub.CatalogService.Application.Services;
using RecordHub.CatalogService.Domain.Models;

namespace RecordHub.CatalogService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesController : ControllerBase
    {
        private readonly ICountryCatalogService _countryService;

        public CountriesController(ICountryCatalogService countryService)
        {
            _countryService = countryService;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CountryModel model, CancellationToken cancellationToken)
        {
            await _countryService.AddAsync(model, cancellationToken);

            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id:Guid}")]
        public async Task<IActionResult> UpdateAsync([FromRoute] Guid id,
           [FromBody] CountryModel model,
           CancellationToken cancellationToken = default)
        {
            await _countryService.UpdateAsync(id, model, cancellationToken);

            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:Guid}")]
        public async Task<IActionResult> DeleteAsync([FromRoute] Guid id,
           CancellationToken cancellationToken = default)
        {
            await _countryService.DeleteAsync(id, cancellationToken);

            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
        {
            var countries = await _countryService.GetAllAsync(cancellationToken);

            return Ok(countries);
        }
    }
}
