using RecordHub.CatalogService.Domain.Entities;

namespace RecordHub.CatalogService.Application.Data.Repositories
{
    public interface IRecordRepository : IRepository<Record>
    {
        Task<int> GetCountAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<Record>> GetByPageAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    }
}
