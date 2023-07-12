using RecordHub.CatalogService.Application.DTO;
using RecordHub.CatalogService.Domain.Models;

namespace RecordHub.CatalogService.Application.Services
{
    public interface IRecordCatalogService
    {
        Task<RecordDTO?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
        Task<RecordDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task AddAsync(RecordModel model, CancellationToken cancellationToken = default);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

        Task UpdateAsync(Guid id, RecordModel model, CancellationToken cancellationToken = default);
        Task<IEnumerable<RecordDTO>> GetByPageAsync(int page, int pageSize, CancellationToken cancellationToken = default);
        Task<int> GetCountAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<RecordDTO>> GetByPageAsync(RecordFilterModel filterModel, CancellationToken cancellationToken = default);
        Task<IEnumerable<RecordDTO>> SearchAsync(string text, CancellationToken cancellationToken = default);
    }
}
