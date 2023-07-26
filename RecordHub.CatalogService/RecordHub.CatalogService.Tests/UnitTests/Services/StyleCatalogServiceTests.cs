using AutoMapper;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Nest;
using RecordHub.CatalogService.Application.Data;
using RecordHub.CatalogService.Application.DTO;
using RecordHub.CatalogService.Domain.Entities;
using RecordHub.CatalogService.Domain.Models;
using RecordHub.CatalogService.Infrastructure.Services;
using RecordHub.CatalogService.Tests.Generators;
using RecordHub.CatalogService.Tests.Setups;
using Record = RecordHub.CatalogService.Domain.Entities.Record;

namespace RecordHub.CatalogService.Tests.UnitTests.Services
{
    public class StyleCatalogServiceTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IElasticClient> _elasticClientMock;
        private readonly Mock<IValidator<BaseEntity>> _validatorMock;
        private readonly StyleCatalogService _styleCatalogService;
        private readonly BaseEntityGenerator _baseEntityGenerator;

        public StyleCatalogServiceTests()
        {
            _baseEntityGenerator = new BaseEntityGenerator();
            _mapperMock = new Mock<IMapper>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _validatorMock = new Mock<IValidator<BaseEntity>>();
            _elasticClientMock = new Mock<IElasticClient>();
            _styleCatalogService = new StyleCatalogService(
                _mapperMock.Object,
                _unitOfWorkMock.Object,
                _validatorMock.Object,
                _elasticClientMock.Object);
        }

        [Fact]
        public async Task AddAsync_ValidModel_AddsStyleAndCommitsUnitOfWork()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var model = _baseEntityGenerator.GenerateStyleModel();

            var style = new Style
            {
                Id = Guid.NewGuid(),
                Name = model.Name,
                Slug = model.Slug,
            };

            _mapperMock.SetupMap(model, style);
            var validResult = new ValidationResult();
            _validatorMock.SetupValidatorMock(validResult, cancellationToken);

            _unitOfWorkMock.SetupUnitOfWorkMockAddStyleAsync(cancellationToken);

            // Act
            await _styleCatalogService.AddAsync(model, cancellationToken);

            // Assert
            _validatorMock.Verify(); // Verify that ValidateAndThrowAsync was called

            _unitOfWorkMock
                .Verify(uow => uow.Styles
                .AddAsync(style, cancellationToken),
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
            var model = _baseEntityGenerator.GenerateStyleModel();

            var style = new Style
            {
                Id = Guid.NewGuid(),
                Name = model.Name,
                Slug = model.Slug
            };

            _mapperMock.SetupMap(model, style);

            var validationException = new ValidationException("Validation error");
            _validatorMock.SetupValidatorMockThrowsException(validationException);

            // Act and Assert
            await _styleCatalogService
                .Invoking(async service => await service
                .AddAsync(model, cancellationToken))
                .Should()
                .ThrowAsync<ValidationException>()
                .WithMessage(validationException.Message);

            _unitOfWorkMock
                .Verify(uow => uow.Styles
                .AddAsync(It.IsAny<Style>(), cancellationToken),
                Times.Never); // Verify AddAsync was not called

            _unitOfWorkMock
                .Verify(uow => uow
                .CommitAsync(cancellationToken),
                Times.Never); // Verify CommitAsync was not called
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllStyles()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var entities = _baseEntityGenerator.GenerateBaseEntities();

            var styles = entities.Select(e => new Style
            {
                Id = e.Id,
                Name = e.Name,
                Slug = e.Slug
            });

            var expectedStyleDTOs = styles.Select(s => new StyleDTO
            {
                Name = s.Name,
                Slug = s.Slug
            });

            _unitOfWorkMock.SetupUnitOfWorkMockGetAllStylesAsync(styles, cancellationToken);
            _mapperMock.SetupMap(styles, expectedStyleDTOs);

            // Act
            var result = await _styleCatalogService.GetAllAsync(cancellationToken);

            // Assert
            result
                .Should()
                .BeEquivalentTo(expectedStyleDTOs);
        }

        [Fact]
        public async Task DeleteAsync_StyleFound_DeletesStyleAndIndexesInElasticsearch()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;

            var baseEntity = _baseEntityGenerator.GenerateBaseEntity();
            var style = new Style
            {
                Id = baseEntity.Id,
                Name = baseEntity.Name,
                Slug = baseEntity.Slug
            };

            _unitOfWorkMock.SetupUnitOfWorkMockStyleDeleteAsync(style, cancellationToken);
            _elasticClientMock.SetupDeleteByQueryAsync(It.IsAny<Func<QueryContainerDescriptor<RecordDTO>, QueryContainer>>());

            // Act
            await _styleCatalogService.DeleteAsync(style.Id, cancellationToken);

            // Assert
            _unitOfWorkMock.Verify();
            _elasticClientMock.Verify();
        }

        [Fact]
        public async Task UpdateAsync_StyleFound_UpdatesStyleAndIndexesRecords()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var styleId = Guid.NewGuid();

            var model = _baseEntityGenerator.GenerateStyleModel();

            var style = new Style
            {
                Id = styleId,
                Name = model.Name,
                Slug = model.Slug,
                Records = new List<Record>
                {
                    new Record { Id = Guid.NewGuid(), Name = "Record 1",  },
                    new Record { Id = Guid.NewGuid(), Name = "Record 2", }
                }
            };

            var recordsDTO = style.Records.Select(r => new RecordDTO { Id = r.Id, Name = r.Name }).ToList();

            _unitOfWorkMock.SetupUnitOfWorkMockGetByIdIncludedGraph(style, cancellationToken);
            _mapperMock.SetupMap(model, style);

            _validatorMock.SetupValidatorMock(new ValidationResult(), cancellationToken);
            _unitOfWorkMock.SetupUnitOfWorkStyleUpdateAsync(cancellationToken);

            _mapperMock.Setup(m => m.Map<IEnumerable<RecordDTO>>(style.Records)).Returns(recordsDTO);

            _elasticClientMock.SetupIndexManyAsync(recordsDTO);

            // Act
            await _styleCatalogService.UpdateAsync(styleId, model, cancellationToken);

            // Assert
            _unitOfWorkMock.Verify();
            _validatorMock.Verify();
            _mapperMock.Verify(m => m.Map(model, style), Times.Once);
            _elasticClientMock.Verify();
        }

        [Fact]
        public async Task UpdateAsync_StyleNotFound_ThrowsArgumentException()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var styleId = Guid.NewGuid();

            _unitOfWorkMock.SetupUnitOfWorkMockGetByIdIncludedGraph(null, cancellationToken);

            // Act
            Func<Task> updateAsyncTask = async () => await _styleCatalogService.UpdateAsync(styleId, new StyleModel(), cancellationToken);

            // Assert
            await updateAsyncTask
                .Should()
                .ThrowAsync<ArgumentException>();
        }
    }
}
