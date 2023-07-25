using AutoMapper;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Nest;
using RecordHub.CatalogService.Application.Data;
using RecordHub.CatalogService.Application.DTO;
using RecordHub.CatalogService.Domain.Entities;
using RecordHub.CatalogService.Infrastructure.Services;
using RecordHub.CatalogService.Tests.Generators;
using RecordHub.CatalogService.Tests.Setups;
using RecordHub.Shared.Exceptions;
using Record = RecordHub.CatalogService.Domain.Entities.Record;

namespace RecordHub.CatalogService.Tests.UnitTests.Services
{
    public class ArtistCatalogServiceTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IElasticClient> _elasticClientMock;
        private readonly Mock<IValidator<BaseEntity>> _validatorMock;
        private readonly ArtistCatalogService _artistCatalogService;
        private readonly ArtistGenerator _artistGenerator;

        public ArtistCatalogServiceTests()
        {
            _artistGenerator = new ArtistGenerator();
            _mapperMock = new Mock<IMapper>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _validatorMock = new Mock<IValidator<BaseEntity>>();
            _elasticClientMock = new Mock<IElasticClient>();
            _artistCatalogService = new ArtistCatalogService(
                _mapperMock.Object,
                _unitOfWorkMock.Object,
                _validatorMock.Object,
                _elasticClientMock.Object);
        }

        [Fact]
        public async Task AddAsync_ValidModel_AddsArtistAndCommitsUnitOfWork()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var model = _artistGenerator.GenerateModel();

            var artist = new Artist
            {
                Id = Guid.NewGuid(),
                Name = model.Name,
                Slug = model.Slug,
            };

            _mapperMock.SetupMap(model, artist);
            var validResult = new ValidationResult();
            _validatorMock.SetupValidatorMock(validResult, cancellationToken);

            _unitOfWorkMock.SetupUnitOfWorkMockAddArtistAsync(cancellationToken);

            // Act
            await _artistCatalogService.AddAsync(model, cancellationToken);

            // Assert
            _validatorMock.Verify(); // Verify that ValidateAndThrowAsync was called

            _unitOfWorkMock
                .Verify(uow => uow.Artists
                .AddAsync(artist, cancellationToken),
                Times.Once); // Verify AddAsync was called

            _unitOfWorkMock
                .Verify(uow => uow
                .CommitAsync(cancellationToken),
                Times.Once); // Verify CommitAsync was called

        }

        [Fact]
        public async Task AddAsync_InvalidModel_ThrowsValidationException()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var model = _artistGenerator.GenerateModel();

            var artist = new Artist
            {
                Id = Guid.NewGuid(),
                Name = model.Name,
                Slug = model.Slug
            };

            _mapperMock.SetupMap(model, artist);

            var validationException = new ValidationException("Validation error");
            _validatorMock.SetupValidatorMockThrowsException(validationException);

            // Act and Assert
            await _artistCatalogService
                .Invoking(async service => await service
                .AddAsync(model, cancellationToken))
                .Should()
                .ThrowAsync<ValidationException>()
                .WithMessage(validationException.Message);

            _unitOfWorkMock
                .Verify(uow => uow.Artists
                .AddAsync(It.IsAny<Artist>(), cancellationToken),
                Times.Never); // Verify AddAsync was not called

            _unitOfWorkMock
                .Verify(uow => uow
                .CommitAsync(cancellationToken),
                Times.Never); // Verify CommitAsync was not called
        }

        [Fact]
        public async Task GetBySlug_ArtistNotFound_ThrowsEntityNotFoundException()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var slug = "sample-slug";

            _unitOfWorkMock.SetupUnitOfWorkMockArtistGetBySlugAsync(null, cancellationToken);

            // Act and Assert
            await _artistCatalogService
                .Invoking(async service => await service
                .GetBySlug(slug, cancellationToken))
                .Should()
                .ThrowAsync<EntityNotFoundException>();
        }

        [Fact]
        public async Task GetBySlug_ArtistFound_ReturnsArtistDTO()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;

            var artist = _artistGenerator.GenerateEntity();

            var expectedArtistDTO = new ArtistDTO
            {
                Slug = artist.Slug,
                Name = artist.Name,
            };

            _unitOfWorkMock.SetupUnitOfWorkMockArtistGetBySlugAsync(artist, cancellationToken);

            _mapperMock.SetupMap(artist, expectedArtistDTO);

            // Act
            var result = await _artistCatalogService.GetBySlug(artist.Slug, cancellationToken);

            // Assert
            result
                .Should()
                .BeEquivalentTo(expectedArtistDTO);
        }


        [Fact]
        public async Task DeleteAsync_ArtistFound_DeletesArtistAndIndexesInElasticsearch()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;

            var artist = _artistGenerator.GenerateEntity();

            _unitOfWorkMock.SetupUnitOfWorkArtistDeleteAsync(artist, cancellationToken);
            _elasticClientMock.SetupDeleteByQueryAsync(It.IsAny<Func<QueryContainerDescriptor<RecordDTO>, QueryContainer>>());

            // Act
            await _artistCatalogService.DeleteAsync(artist.Id, cancellationToken);

            // Assert
            _unitOfWorkMock.Verify();
            _elasticClientMock.Verify();
        }

        [Fact]
        public async Task UpdateAsync_ArtistFound_UpdatesArtistAndIndexesRecords()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var artistId = Guid.NewGuid();

            var model = _artistGenerator.GenerateModel();

            var artist = new Artist
            {
                Id = artistId,
                Name = model.Name,
                Slug = model.Slug
            };

            var records = new List<Record>
            {
                new Record { Id = Guid.NewGuid(), Name = "Record 1", ArtistId = artistId },
                new Record { Id = Guid.NewGuid(), Name = "Record 2", ArtistId = artistId }
            };

            var recordsDTO = records.Select(r => new RecordDTO { Id = r.Id, Name = r.Name }).ToList();

            _unitOfWorkMock.SetupUnitOfWorkArtistGetByIdAsync(artist, cancellationToken);
            _mapperMock.SetupMap(model, artist);

            _validatorMock.SetupValidatorMock(new ValidationResult(), cancellationToken);
            _unitOfWorkMock.SetupUnitOfWorkArtistUpdateAsync(artist, cancellationToken);
            _unitOfWorkMock.SetupGetArtistsRecordsAsync(records, cancellationToken);

            _mapperMock.Setup(m => m.Map<IEnumerable<RecordDTO>>(records)).Returns(recordsDTO);

            _elasticClientMock.SetupIndexManyAsync(recordsDTO);

            // Act
            await _artistCatalogService.UpdateAsync(artistId, model, cancellationToken);

            // Assert
            _unitOfWorkMock.Verify();
            _validatorMock.Verify();
            _mapperMock.Verify(m => m.Map(model, artist), Times.Once);
            _elasticClientMock.Verify();
        }
    }
}
