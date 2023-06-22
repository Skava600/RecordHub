using RecordHub.CatalogService.Application.DTO;
using RecordHub.CatalogService.Domain.Models;

namespace RecordHub.CatalogService.Application.Services
{
    public interface IStyleCatalogService
    {
        Task UpdateAsync(Guid id, StyleModel model, CancellationToken cancellationToken);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken);
        Task AddAsync(StyleModel model, CancellationToken cancellationToken);
        Task<IEnumerable<StyleDTO>> GetAllAsync(CancellationToken cancellationToken);
    }
}
