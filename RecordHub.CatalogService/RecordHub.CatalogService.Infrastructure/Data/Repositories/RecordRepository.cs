using Microsoft.EntityFrameworkCore;
using RecordHub.CatalogService.Application.Data.Repositories;
using RecordHub.CatalogService.Domain.Entities;

namespace RecordHub.CatalogService.Infrastructure.Data.Repositories
{
    public class RecordRepository : BaseRepository<Record>, IRecordRepository
    {
        private readonly DbSet<Record> records;

        public RecordRepository(ApplicationDbContext context)
            : base(context)
        {
            records = context.Records;
        }

        public Task<int> GetCountAsync(CancellationToken cancellationToken = default)
        {
            return records
                .AsNoTracking()
                .CountAsync(cancellationToken);
        }

        public override async Task<Record?> GetBySlugAsync(
            string slug,
            CancellationToken cancellationToken = default)
        {
            return await GetAllQueryIncludeGraph()
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Slug.Equals(slug, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<IEnumerable<Record>> GetByPageAsync(
            int page,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            return await GetAllQueryIncludeGraph()
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<Record?> GetByIdGraphIncludedAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await GetAllQueryIncludeGraph()
                .Where(r => r.Id.Equals(id)).FirstAsync(cancellationToken);
        }

        public async Task<IEnumerable<Record>> GetArtistsRecordsAsync(
            Guid artistId,
            CancellationToken cancellationToken = default)
        {
            return await GetAllQueryIncludeGraph()
                .Where(r => r.ArtistId.Equals(artistId))
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Record>> GetCountrysRecordsAsync(
            Guid countryId,
            CancellationToken cancellationToken = default)
        {
            return await GetAllQueryIncludeGraph()
              .Where(r => r.CountryId.Equals(countryId))
              .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Record>> GetLabelsRecordsAsync(
            Guid labelId,
            CancellationToken cancellationToken = default)
        {
            return await GetAllQueryIncludeGraph()
             .Where(r => r.LabelId.Equals(labelId))
             .ToListAsync(cancellationToken);
        }

        private IQueryable<Record> GetAllQueryIncludeGraph()
        {
            return records
                .Include(r => r.Label)
                .Include(r => r.Country)
                .Include(r => r.Styles)
                .Include(r => r.Artist)
                .AsSplitQuery();
        }
    }
}
