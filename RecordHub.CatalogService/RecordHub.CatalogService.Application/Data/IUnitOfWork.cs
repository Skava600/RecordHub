using RecordHub.CatalogService.Application.Data.Repositories;
using RecordHub.CatalogService.Domain.Entities;

namespace RecordHub.CatalogService.Application.Data
{
    public interface IUnitOfWork
    {
        IRecordRepository Records { get; }
        IRepository<Label> Labels { get; }
        IArtistRepository Artists { get; }
        IStyleRepository Styles { get; }
        IRepository<Country> Countries { get; }
        Task CommitAsync(CancellationToken cancellationToken = default);
    }
}
