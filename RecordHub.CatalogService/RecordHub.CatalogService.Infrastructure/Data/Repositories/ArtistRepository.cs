using Microsoft.EntityFrameworkCore;
using RecordHub.CatalogService.Application.Data.Repositories;
using RecordHub.CatalogService.Domain.Entities;

namespace RecordHub.CatalogService.Infrastructure.Data.Repositories
{
    public class ArtistRepository : BaseRepository<Artist>, IArtistRepository
    {
        private readonly DbSet<Artist> artists;

        public ArtistRepository(ApplicationDbContext context)
            : base(context)
        {
            artists = context.Artists;
        }

        public override async Task<Artist?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
        {
            return await artists
                .Include(a => a.Records)
                    .ThenInclude(r => r.Styles)
                .AsSplitQuery()
                .FirstOrDefaultAsync(e => e.Slug.Equals(slug), cancellationToken);
        }
    }
}
