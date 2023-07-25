using RecordHub.CatalogService.Application.Data;
using RecordHub.CatalogService.Domain.Entities;
using Record = RecordHub.CatalogService.Domain.Entities.Record;

namespace RecordHub.CatalogService.Tests.Setups
{
    public static class UnitOfWorkExtensions
    {
        public static void SetupUnitOfWorkMockAddRecordAsync(this Mock<IUnitOfWork> unitOfWorkMock, CancellationToken cancellationToken = default)
        {
            unitOfWorkMock
                .Setup(uow => uow.Records.AddAsync(It.IsAny<Record>(), cancellationToken))
                .Verifiable();

            unitOfWorkMock.SetupUnitOfWorkMockCommitAsync(cancellationToken);
        }

        public static void SetupUnitOfWorkMockAddLabelAsync(this Mock<IUnitOfWork> unitOfWorkMock, CancellationToken cancellationToken = default)
        {
            unitOfWorkMock
                .Setup(uow => uow.Records.AddAsync(It.IsAny<Record>(), cancellationToken))
                .Verifiable();

            unitOfWorkMock.SetupUnitOfWorkMockCommitAsync(cancellationToken);
        }

        public static void SetupUnitOfWorkMockAddCountryAsync(this Mock<IUnitOfWork> unitOfWorkMock, CancellationToken cancellationToken = default)
        {
            unitOfWorkMock
               .Setup(uow => uow.Countries.AddAsync(It.IsAny<Country>(), cancellationToken))
               .Verifiable();

            unitOfWorkMock.SetupUnitOfWorkMockCommitAsync(cancellationToken);
        }

        public static void SetupUnitOfWorkMockAddArtistAsync(this Mock<IUnitOfWork> unitOfWorkMock, CancellationToken cancellationToken = default)
        {
            unitOfWorkMock
                .Setup(uow => uow.Artists.AddAsync(It.IsAny<Artist>(), cancellationToken))
                .Verifiable();

            unitOfWorkMock.SetupUnitOfWorkMockCommitAsync(cancellationToken);
        }

        public static void SetupUnitOfWorkMockRecordDeleteAsync(this Mock<IUnitOfWork> unitOfWorkMock, Record deletedRecord, CancellationToken cancellationToken = default)
        {
            unitOfWorkMock
                .Setup(repo => repo
                .Records.DeleteAsync(It.IsAny<Guid>(), cancellationToken))
                .ReturnsAsync(deletedRecord)
                .Verifiable();

            unitOfWorkMock.SetupUnitOfWorkMockCommitAsync(cancellationToken);
        }

        public static void SetupUnitOfWorkArtistDeleteAsync(this Mock<IUnitOfWork> unitOfWorkMock, Artist artist, CancellationToken cancellationToken = default)
        {
            unitOfWorkMock
                .Setup(repo => repo
                .Artists.DeleteAsync(artist.Id, cancellationToken))
                .ReturnsAsync(artist)
                .Verifiable();

            unitOfWorkMock.SetupUnitOfWorkMockCommitAsync(cancellationToken);
        }

        public static void SetupUnitOfWorkArtistGetByIdAsync(this Mock<IUnitOfWork> unitOfWorkMock, Artist artist, CancellationToken cancellationToken)
        {
            unitOfWorkMock
                .Setup(repo => repo.Artists.GetByIdAsync(It.IsAny<Guid>(), cancellationToken))
                .ReturnsAsync(artist)
                .Verifiable();
        }

        public static void SetupUnitOfWorkMockRecordGetBySlugAsync(this Mock<IUnitOfWork> unitOfWorkMock, Record? record, CancellationToken cancellationToken = default)
        {
            unitOfWorkMock
               .Setup(repo => repo.Records.GetBySlugAsync(It.IsAny<string>(), cancellationToken))
               .ReturnsAsync(record)
               .Verifiable();
        }

        public static void SetupUnitOfWorkMockArtistGetBySlugAsync(this Mock<IUnitOfWork> unitOfWorkMock, Artist? artist, CancellationToken cancellationToken = default)
        {
            unitOfWorkMock
               .Setup(repo => repo.Artists.GetBySlugAsync(It.IsAny<string>(), cancellationToken))
               .ReturnsAsync(artist)
               .Verifiable();
        }

        public static void SetupGetByIdGraphIncludedAsync(this Mock<IUnitOfWork> unitOfWorkMock, Record? record, CancellationToken cancellationToken = default)
        {
            unitOfWorkMock.Setup(repo => repo.Records.GetByIdGraphIncludedAsync(It.IsAny<Guid>(), cancellationToken))
                .ReturnsAsync(record)
                .Verifiable();
        }

        public static void SetupUnitOfWorkRecordUpdateAsync(this Mock<IUnitOfWork> unitOfWorkMock, Record record, CancellationToken cancellationToken = default)
        {
            unitOfWorkMock.Setup(repo => repo.Records.UpdateAsync(record, cancellationToken))
                .Returns(Task.CompletedTask)
                .Verifiable();

            unitOfWorkMock.SetupUnitOfWorkMockCommitAsync(cancellationToken);
        }
        public static void SetupUnitOfWorkArtistUpdateAsync(this Mock<IUnitOfWork> unitOfWorkMock, Artist artist, CancellationToken cancellationToken = default)
        {
            unitOfWorkMock.Setup(repo => repo.Artists.UpdateAsync(artist, cancellationToken))
                .Returns(Task.CompletedTask)
                .Verifiable();

            unitOfWorkMock.SetupUnitOfWorkMockCommitAsync(cancellationToken);
        }

        public static void SetupGetArtistsRecordsAsync(this Mock<IUnitOfWork> unitOfWorkMock, IEnumerable<Record> records, CancellationToken cancellationToken = default)
        {
            unitOfWorkMock
                .Setup(repo => repo.Records.GetArtistsRecordsAsync(It.IsAny<Guid>(), cancellationToken))
                .ReturnsAsync(records)
                .Verifiable();
        }

        private static void SetupUnitOfWorkMockCommitAsync(this Mock<IUnitOfWork> unitOfWorkMock, CancellationToken cancellationToken = default)
        {
            unitOfWorkMock
               .Setup(uow => uow.CommitAsync(cancellationToken))
               .Verifiable();
        }
    }
}
