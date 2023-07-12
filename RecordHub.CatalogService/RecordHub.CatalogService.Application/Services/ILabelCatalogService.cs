using RecordHub.CatalogService.Application.DTO;
using RecordHub.CatalogService.Domain.Models;

namespace RecordHub.CatalogService.Application.Services
{
    public interface ILabelCatalogService
    {
        Task UpdateAsync(Guid id, LabelModel model, CancellationToken cancellationToken);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken);
        Task AddAsync(LabelModel model, CancellationToken cancellationToken);
        Task<IEnumerable<LabelDTO>> GetAllAsync(CancellationToken cancellationToken);
    }
}
