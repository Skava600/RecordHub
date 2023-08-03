using RecordHub.CatalogService.Application.Data;
using RecordHub.CatalogService.Domain.Entities;
using Record = RecordHub.CatalogService.Domain.Entities.Record;

namespace RecordHub.CatalogService.Tests.Setups
{
    public static class UnitOfWorkExtensions
    {
        //Setup record repository
        public static void SetupUnitOfWorkMockAddRecordAsync(this Mock<IUnitOfWork> unitOfWorkMock, CancellationToken cancellationToken = default)
        {
            unitOfWorkMock
                .Setup(uow => uow.Records.AddAsync(It.IsAny<Record>(), cancellationToken))
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

        public static void SetupUnitOfWorkRecordUpdateAsync(this Mock<IUnitOfWork> unitOfWorkMock, CancellationToken cancellationToken = default)
        {
            unitOfWorkMock.Setup(repo => repo.Records.UpdateAsync(It.IsAny<Record>(), cancellationToken))
                .Returns(Task.CompletedTask)
                .Verifiable();

            unitOfWorkMock.SetupUnitOfWorkMockCommitAsync(cancellationToken);
        }

        public static void SetupUnitOfWorkMockRecordGetBySlugAsync(this Mock<IUnitOfWork> unitOfWorkMock, Record? record, CancellationToken cancellationToken = default)
        {
            unitOfWorkMock
               .Setup(repo => repo.Records.GetBySlugAsync(It.IsAny<string>(), cancellationToken))
               .ReturnsAsync(record)
               .Verifiable();
        }

        public static void SetupGetArtistsRecordsAsync(this Mock<IUnitOfWork> unitOfWorkMock, IEnumerable<Record> records, CancellationToken cancellationToken = default)
        {
            unitOfWorkMock
                .Setup(repo => repo.Records.GetArtistsRecordsAsync(It.IsAny<Guid>(), cancellationToken))
                .ReturnsAsync(records)
                .Verifiable();
        }

        public static void SetupGetByIdRecordGraphIncludedAsync(this Mock<IUnitOfWork> unitOfWorkMock, Record? record, CancellationToken cancellationToken = default)
        {
            unitOfWorkMock.Setup(repo => repo.Records.GetByIdGraphIncludedAsync(It.IsAny<Guid>(), cancellationToken))
                .ReturnsAsync(record)
                .Verifiable();
        }
        public static void SetupGetLabelsRecordsAsync(this Mock<IUnitOfWork> unitOfWorkMock, IEnumerable<Record> records, CancellationToken cancellationToken)
        {
            unitOfWorkMock
                .Setup(uow => uow.Records.GetLabelsRecordsAsync(It.IsAny<Guid>(), cancellationToken))
                .ReturnsAsync(records);
        }

        public static void SetupGetCountriesRecordsAsync(this Mock<IUnitOfWork> unitOfWorkMock, IEnumerable<Record> records, CancellationToken cancellationToken)
        {
            unitOfWorkMock
                .Setup(uow => uow.Records.GetCountrysRecordsAsync(It.IsAny<Guid>(), cancellationToken))
                .ReturnsAsync(records);
        }

        //Setup label repository
        public static void SetupUnitOfWorkMockAddLabelAsync(this Mock<IUnitOfWork> unitOfWorkMock, CancellationToken cancellationToken = default)
        {
            unitOfWorkMock
                .Setup(uow => uow.Labels.AddAsync(It.IsAny<Label>(), cancellationToken))
                .Verifiable();

            unitOfWorkMock.SetupUnitOfWorkMockCommitAsync(cancellationToken);
        }

        public static void SetupUnitOfWorkLabelGetByIdAsync(this Mock<IUnitOfWork> unitOfWorkMock, Label label, CancellationToken cancellationToken)
        {
            unitOfWorkMock
                .Setup(uow => uow.Labels.GetByIdAsync(It.IsAny<Guid>(), cancellationToken))
                .ReturnsAsync(label);
        }

        public static void SetupUnitOfWorkMockLabelDeleteAsync(this Mock<IUnitOfWork> unitOfWorkMock, Label deletedLabel, CancellationToken cancellationToken = default)
        {
            unitOfWorkMock
                .Setup(repo => repo
                .Labels.DeleteAsync(It.IsAny<Guid>(), cancellationToken))
                .ReturnsAsync(deletedLabel)
                .Verifiable();

            unitOfWorkMock.SetupUnitOfWorkMockCommitAsync(cancellationToken);
        }

        public static void SetupUnitOfWorkLabelUpdateAsync(this Mock<IUnitOfWork> unitOfWorkMock, CancellationToken cancellationToken)
        {
            unitOfWorkMock
                .Setup(uow => uow.Labels.UpdateAsync(It.IsAny<Label>(), cancellationToken))
                .Returns(Task.CompletedTask);
        }

        public static void SetupUnitOfWorkMockGetAllLabelsAsync(this Mock<IUnitOfWork> unitOfWorkMock, IEnumerable<Label> labels, CancellationToken cancellationToken)
        {
            unitOfWorkMock
                .Setup(uow => uow.Labels.GetAllAsync(cancellationToken))
                .ReturnsAsync(labels);
        }

        // Setup country repository
        public static void SetupUnitOfWorkMockAddCountryAsync(this Mock<IUnitOfWork> unitOfWorkMock, CancellationToken cancellationToken = default)
        {
            unitOfWorkMock
               .Setup(uow => uow.Countries.AddAsync(It.IsAny<Country>(), cancellationToken))
               .Verifiable();

            unitOfWorkMock.SetupUnitOfWorkMockCommitAsync(cancellationToken);
        }

        public static void SetupUnitOfWorkMockCountryDeleteAsync(this Mock<IUnitOfWork> unitOfWorkMock, Country deletedCountry, CancellationToken cancellationToken = default)
        {
            unitOfWorkMock
                .Setup(repo => repo
                .Countries.DeleteAsync(It.IsAny<Guid>(), cancellationToken))
                .ReturnsAsync(deletedCountry)
                .Verifiable();

            unitOfWorkMock.SetupUnitOfWorkMockCommitAsync(cancellationToken);
        }

        public static void SetupUnitOfWorkMockGetAllCountriesAsync(this Mock<IUnitOfWork> unitOfWorkMock, IEnumerable<Country> countries, CancellationToken cancellationToken)
        {
            unitOfWorkMock
                .Setup(uow => uow.Countries.GetAllAsync(cancellationToken))
                .ReturnsAsync(countries.ToList());
        }

        public static void SetupUnitOfWorkMockCountryGetByIdAsync(this Mock<IUnitOfWork> unitOfWorkMock, Country country, CancellationToken cancellationToken)
        {
            unitOfWorkMock
                .Setup(uow => uow.Countries.GetByIdAsync(It.IsAny<Guid>(), cancellationToken))
                .ReturnsAsync(country);
        }

        public static void SetupUnitOfWorkCountryUpdateAsync(this Mock<IUnitOfWork> unitOfWorkMock, CancellationToken cancellationToken)
        {
            unitOfWorkMock
                .Setup(uow => uow.Countries.UpdateAsync(It.IsAny<Country>(), cancellationToken))
                .Returns(Task.CompletedTask);
        }


        //Setup artist repository
        public static void SetupUnitOfWorkMockAddArtistAsync(this Mock<IUnitOfWork> unitOfWorkMock, CancellationToken cancellationToken = default)
        {
            unitOfWorkMock
                .Setup(uow => uow.Artists.AddAsync(It.IsAny<Artist>(), cancellationToken))
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

        public static void SetupUnitOfWorkMockArtistGetBySlugAsync(this Mock<IUnitOfWork> unitOfWorkMock, Artist? artist, CancellationToken cancellationToken = default)
        {
            unitOfWorkMock
               .Setup(repo => repo.Artists.GetBySlugAsync(It.IsAny<string>(), cancellationToken))
               .ReturnsAsync(artist)
               .Verifiable();
        }

        public static void SetupUnitOfWorkArtistUpdateAsync(this Mock<IUnitOfWork> unitOfWorkMock, CancellationToken cancellationToken = default)
        {
            unitOfWorkMock.Setup(repo => repo.Artists.UpdateAsync(It.IsAny<Artist>(), cancellationToken))
                .Returns(Task.CompletedTask)
                .Verifiable();

            unitOfWorkMock.SetupUnitOfWorkMockCommitAsync(cancellationToken);
        }

        //Setup styles repository
        public static void SetupUnitOfWorkMockAddStyleAsync(this Mock<IUnitOfWork> unitOfWorkMock, CancellationToken cancellationToken)
        {
            unitOfWorkMock
                .Setup(uow => uow.Styles.AddAsync(It.IsAny<Style>(), cancellationToken))
                .Returns(Task.CompletedTask)
                .Verifiable();
        }

        public static void SetupUnitOfWorkMockGetAllStylesAsync(this Mock<IUnitOfWork> unitOfWorkMock, IEnumerable<Style> styles, CancellationToken cancellationToken)
        {
            unitOfWorkMock
                .Setup(uow => uow.Styles.GetAllAsync(cancellationToken))
                .ReturnsAsync(styles)
                .Verifiable();
        }

        public static void SetupUnitOfWorkMockStyleDeleteAsync(this Mock<IUnitOfWork> unitOfWorkMock, Style deletedStyle, CancellationToken cancellationToken = default)
        {
            unitOfWorkMock
                .Setup(repo => repo
                .Styles.DeleteAsync(It.IsAny<Guid>(), cancellationToken))
                .ReturnsAsync(deletedStyle)
                .Verifiable();

            unitOfWorkMock.SetupUnitOfWorkMockCommitAsync(cancellationToken);
        }

        public static void SetupUnitOfWorkMockGetByIdIncludedGraph(this Mock<IUnitOfWork> unitOfWorkMock, Style style, CancellationToken cancellationToken)
        {
            unitOfWorkMock
                .Setup(uow => uow.Styles.GetByIdIncludedGraph(It.IsAny<Guid>(), cancellationToken))
                .ReturnsAsync(style)
                .Verifiable();
        }

        public static void SetupUnitOfWorkStyleUpdateAsync(this Mock<IUnitOfWork> unitOfWorkMock, CancellationToken cancellationToken)
        {
            unitOfWorkMock
                .Setup(uow => uow.Styles.UpdateAsync(It.IsAny<Style>(), cancellationToken))
                .Returns(Task.CompletedTask)
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
