using RecordHub.CatalogService.Application.Data;
using RecordHub.CatalogService.Application.Data.Repositories;
using RecordHub.CatalogService.Infrastructure.Data.Repositories;

namespace RecordHub.CatalogService.Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }
        public IRecordRepository Records => new RecordRepository(_context);

        public ILabelRepository Labels => new LabelRepository(_context);

        public IArtistRepository Artists => new ArtistRepository(_context);

        public IStyleRepository Styles => new StyleRepository(_context);

        public ICountryRepository Countries => new CountryRepository(_context);

        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync();
        }
    }
}
