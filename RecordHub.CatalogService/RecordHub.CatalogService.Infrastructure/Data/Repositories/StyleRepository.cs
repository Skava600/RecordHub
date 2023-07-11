using Microsoft.EntityFrameworkCore;
using RecordHub.CatalogService.Application.Data.Repositories;
using RecordHub.CatalogService.Domain.Entities;

namespace RecordHub.CatalogService.Infrastructure.Data.Repositories
{
    public class StyleRepository : BaseRepository<Style>, IStyleRepository
    {
        private readonly DbSet<Style> _styles;
        public StyleRepository(ApplicationDbContext context) : base(context)
        {
            _styles = context.Styles;
        }

        public async Task<Style?> GetByIdIncludedGraph(Guid id, CancellationToken cancellationToken)
        {
            return await _styles.Include(s => s.Records).ThenInclude(r => r.Artist).FirstOrDefaultAsync(s => s.Id.Equals(id));
        }
    }
}
