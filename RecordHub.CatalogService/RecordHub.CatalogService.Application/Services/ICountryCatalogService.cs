using RecordHub.CatalogService.Application.DTO;
using RecordHub.CatalogService.Domain.Models;

namespace RecordHub.CatalogService.Application.Services
{
    public interface ICountryCatalogService
    {
        Task UpdateAsync(Guid id, CountryModel model, CancellationToken cancellationToken);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken);
        Task AddAsync(CountryModel model, CancellationToken cancellationToken);
        Task<IEnumerable<CountryDTO>> GetAllAsync(CancellationToken cancellationToken);
    }
}
