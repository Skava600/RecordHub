using RecordHub.CatalogService.Application.Data;
using RecordHub.CatalogService.Application.Data.Repositories;
using RecordHub.CatalogService.Domain.Entities;
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

        public IRepository<Label> Labels => new BaseRepository<Label>(_context);

        public IRepository<Artist> Artists => new BaseRepository<Artist>(_context);

        public IRepository<Style> Styles => new BaseRepository<Style>(_context);

        public IRepository<Country> Countries => new BaseRepository<Country>(_context);

        public async Task CommitAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
