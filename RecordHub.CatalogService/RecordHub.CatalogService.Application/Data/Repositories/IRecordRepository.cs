using RecordHub.CatalogService.Domain.Entities;

namespace RecordHub.CatalogService.Application.Data.Repositories
{
    public interface IRecordRepository : IRepository<Record>
    {
        Task<int> GetCountAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<Record>> GetByPageAsync(int page, int pageSize, CancellationToken cancellationToken = default);
        Task<Record?> GetByIdGraphIncludedAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Record>> GetArtistsRecordsAsync(Guid artistId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Record>> GetCountrysRecordsAsync(Guid countryId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Record>> GetLabelsRecordsAsync(Guid labelId, CancellationToken cancellationToken = default);
    }
}
