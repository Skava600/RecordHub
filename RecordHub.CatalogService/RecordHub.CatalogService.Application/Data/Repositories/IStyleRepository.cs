using RecordHub.CatalogService.Domain.Entities;

namespace RecordHub.CatalogService.Application.Data.Repositories
{
    public interface IStyleRepository : IRepository<Style>
    {
        Task<Style?> GetByIdIncludedGraph(Guid id, CancellationToken cancellationToken);
    }
}
