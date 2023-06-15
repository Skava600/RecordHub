using Microsoft.EntityFrameworkCore;
using RecordHub.CatalogService.Application.Data.Repositories;
using RecordHub.CatalogService.Domain.Entities;

namespace RecordHub.CatalogService.Infrastructure.Data.Repositories
{
    public class RecordRepository : BaseRepository<Record>, IRecordRepository
    {
        private readonly DbSet<Record> records;
        public RecordRepository(ApplicationDbContext context) : base(context)
        {
            records = context.Records;
        }

        public Task<int> GetCountAsync(CancellationToken cancellationToken = default)
        {
            return records.AsNoTracking().CountAsync(cancellationToken);
        }

        public async Task<IEnumerable<Record>> GetByPageAsync(int page, int pageSize, CancellationToken cancellationToken = default)
        {
            return await GetAllQueryIncludeGraph()
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        private IQueryable<Record> GetAllQueryIncludeGraph()
        {
            return records
                .Include(r => r.Label)
                .Include(r => r.Country)
                .Include(r => r.Styles)
                .Include(r => r.Artist)
                .AsSplitQuery()
                .AsNoTracking();
        }
    }
}
