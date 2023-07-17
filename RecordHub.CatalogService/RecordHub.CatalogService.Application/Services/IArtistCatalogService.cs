using RecordHub.CatalogService.Application.DTO;
using RecordHub.CatalogService.Domain.Models;

namespace RecordHub.CatalogService.Application.Services
{
    public interface IArtistCatalogService
    {
        Task UpdateAsync(
            Guid id,
            ArtistModel model,
            CancellationToken cancellationToken);

        Task DeleteAsync(Guid id, CancellationToken cancellationToken);

        Task AddAsync(ArtistModel model, CancellationToken cancellationToken);

        Task<ArtistDTO> GetBySlug(string slug, CancellationToken cancellationToken);
    }
}
