using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Nest;
using RecordHub.CatalogService.Application.Data;
using RecordHub.CatalogService.Application.DTO;
using RecordHub.CatalogService.Domain.Entities;
using RecordHub.CatalogService.Domain.Models;
using RecordHub.CatalogService.Infrastructure.Services;
using RecordHub.Shared.Exceptions;

namespace RecordHub.CatalogService.Tests
{
    public class ArtistCatalogServiceTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IValidator<BaseEntity>> _validatorMock;
        private readonly ArtistCatalogService _artistCatalogService;

        public ArtistCatalogServiceTests()
        {
            _mapperMock = new Mock<IMapper>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _validatorMock = new Mock<IValidator<BaseEntity>>();

            _artistCatalogService = new ArtistCatalogService(
                _mapperMock.Object,
                _unitOfWorkMock.Object,
                _validatorMock.Object,
                It.IsAny<IElasticClient>());
        }

        [Fact]
        public async Task AddAsync_ValidModel_AddsArtistAndCommitsUnitOfWork()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var model = new ArtistModel
            {
                Name = "Sample Artist",
                Slug = "sample-artist"
            };

            var artist = new Artist
            {
                Id = Guid.NewGuid(),
                Name = model.Name,
                Slug = model.Slug,
            };

            _mapperMock.Setup(m => m.Map<Artist>(model)).Returns(artist);
            var validResult = new ValidationResult();
            _validatorMock
               .Setup(_ => _.ValidateAsync(It.IsAny<IValidationContext>(), cancellationToken))
               .ReturnsAsync(validResult)
               .Verifiable();

            _unitOfWorkMock.Setup(uow => uow.Artists.AddAsync(artist, cancellationToken)).Verifiable();
            _unitOfWorkMock.Setup(uow => uow.CommitAsync(cancellationToken)).Verifiable();

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
            var model = new ArtistModel
            {
                Name = "not valid name",
                Slug = "not-valid-slug"
            };

            var artist = new Artist
            {
                Id = Guid.NewGuid(),
                Name = "not valid name",
                Slug = "not-valid-slug"
            };

            _mapperMock.Setup(m => m.Map<Artist>(model)).Returns(artist);
            _validatorMock
               .Setup(v => v
               .ValidateAsync(It.IsAny<IValidationContext>(), cancellationToken))
               .Throws(new ValidationException("Validation failed"));

            // Act and Assert
            await Assert.ThrowsAsync<ValidationException>(() => _artistCatalogService.AddAsync(model, cancellationToken));
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

            _unitOfWorkMock
                .Setup(r => r.Artists
                .GetBySlugAsync(slug, cancellationToken))
                .ReturnsAsync((Artist)null);

            // Act and Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _artistCatalogService.GetBySlug(slug, cancellationToken));
        }

        [Fact]
        public async Task GetBySlug_ArtistFound_ReturnsArtistDTO()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var slug = "sample-slug";

            // Create a sample artist
            var artistId = Guid.NewGuid();
            var artist = new Artist
            {
                Id = artistId,
                Slug = slug,
                Name = "Sample Artist"
            };

            var expectedArtistDTO = new ArtistDTO
            {
                Slug = slug,
                Name = "Sample Artist",
                Records = new List<RecordSummaryDTO>()
                {
                    new RecordSummaryDTO
                    {
                        Name = "Name",
                        Slug = "Slug",
                        Price = 100,
                        Styles = new List<StyleDTO>{new StyleDTO { Name ="Rock", Slug = "rock"} }
                    }
                }
            };

            _unitOfWorkMock
                .Setup(r => r.Artists
                .GetBySlugAsync(slug, cancellationToken))
                .ReturnsAsync(artist);

            _mapperMock
                .Setup(m => m.Map<ArtistDTO>(artist))
                .Returns(expectedArtistDTO);

            // Act
            var result = await _artistCatalogService.GetBySlug(slug, cancellationToken);

            // Assert
            Assert.Equal(expectedArtistDTO, result);
        }

    }
}
