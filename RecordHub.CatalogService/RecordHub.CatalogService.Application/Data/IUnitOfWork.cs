using RecordHub.CatalogService.Application.Data.Repositories;

namespace RecordHub.CatalogService.Application.Data
{
    public interface IUnitOfWork
    {
        IRecordRepository Records { get; }

        ILabelRepository Labels { get; }

        IArtistRepository Artists { get; }

        IStyleRepository Styles { get; }

        ICountryRepository Countries { get; }

        Task CommitAsync(CancellationToken cancellationToken = default);
    }
}
