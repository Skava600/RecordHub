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
    public class LabelCatalogServiceTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IElasticClient> _elasticClientMock;
        private readonly Mock<IValidator<BaseEntity>> _validatorMock;
        private readonly LabelCatalogService _labelCatalogService;
        private readonly BaseEntityGenerator _baseEntityGenerator;

        public LabelCatalogServiceTests()
        {
            _baseEntityGenerator = new BaseEntityGenerator();
            _mapperMock = new Mock<IMapper>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _validatorMock = new Mock<IValidator<BaseEntity>>();
            _elasticClientMock = new Mock<IElasticClient>();
            _labelCatalogService = new LabelCatalogService(
                _mapperMock.Object,
                _unitOfWorkMock.Object,
                _validatorMock.Object,
                _elasticClientMock.Object);
        }

        [Fact]
        public async Task AddAsync_ValidModel_AddsLabelAndCommitsUnitOfWork()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var model = _baseEntityGenerator.GenerateLabelModel();

            var label = new Label
            {
                Id = Guid.NewGuid(),
                Name = model.Name,
                Slug = model.Slug,
            };

            _mapperMock.SetupMap(model, label);
            var validResult = new ValidationResult();
            _validatorMock.SetupValidatorMock(validResult, cancellationToken);

            _unitOfWorkMock.SetupUnitOfWorkMockAddLabelAsync(cancellationToken);

            // Act
            await _labelCatalogService.AddAsync(model, cancellationToken);

            // Assert
            _validatorMock.Verify(); // Verify that ValidateAndThrowAsync was called

            _unitOfWorkMock
                .Verify(uow => uow.Labels
                .AddAsync(label, cancellationToken),
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
            var model = _baseEntityGenerator.GenerateLabelModel();

            var label = new Label
            {
                Id = Guid.NewGuid(),
                Name = model.Name,
                Slug = model.Slug
            };

            _mapperMock.SetupMap(model, label);

            var validationException = new ValidationException("Validation error");
            _validatorMock.SetupValidatorMockThrowsException(validationException);

            // Act and Assert
            await _labelCatalogService
                .Invoking(async service => await service
                .AddAsync(model, cancellationToken))
                .Should()
                .ThrowAsync<ValidationException>()
                .WithMessage(validationException.Message);

            _unitOfWorkMock
                .Verify(uow => uow.Labels
                .AddAsync(It.IsAny<Label>(), cancellationToken),
                Times.Never); // Verify AddAsync was not called

            _unitOfWorkMock
                .Verify(uow => uow
                .CommitAsync(cancellationToken),
                Times.Never); // Verify CommitAsync was not called
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllLabels()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var entities = _baseEntityGenerator.GenerateBaseEntities();

            var labels = entities.Select(e => new Label
            {
                Id = e.Id,
                Name = e.Name,
                Slug = e.Slug
            });

            var expectedLabelDTOs = labels.Select(l => new LabelDTO
            {
                Name = l.Name,
                Slug = l.Slug
            });

            _unitOfWorkMock.SetupUnitOfWorkMockGetAllLabelsAsync(labels, cancellationToken);
            _mapperMock.SetupMap(labels, expectedLabelDTOs);

            // Act
            var result = await _labelCatalogService.GetAllAsync(cancellationToken);

            // Assert
            result
                .Should()
                .BeEquivalentTo(expectedLabelDTOs);
        }

        [Fact]
        public async Task DeleteAsync_LabelFound_DeletesLabelAndIndexesInElasticsearch()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;

            var baseEntity = _baseEntityGenerator.GenerateBaseEntity();
            var label = new Label
            {
                Id = baseEntity.Id,
                Name = baseEntity.Name,
                Slug = baseEntity.Slug
            };

            _unitOfWorkMock.SetupUnitOfWorkMockLabelDeleteAsync(label, cancellationToken);
            _elasticClientMock.SetupDeleteByQueryAsync(It.IsAny<Func<QueryContainerDescriptor<RecordDTO>, QueryContainer>>());

            // Act
            await _labelCatalogService.DeleteAsync(label.Id, cancellationToken);

            // Assert
            _unitOfWorkMock.Verify();
            _elasticClientMock.Verify();
        }

        [Fact]
        public async Task UpdateAsync_LabelFound_UpdatesLabelAndIndexesRecords()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var labelId = Guid.NewGuid();

            var model = _baseEntityGenerator.GenerateLabelModel();

            var label = new Label
            {
                Id = labelId,
                Name = model.Name,
                Slug = model.Slug
            };

            var records = new List<Record>
            {
                new Record { Id = Guid.NewGuid(), Name = "Record 1", LabelId = labelId },
                new Record { Id = Guid.NewGuid(), Name = "Record 2", LabelId = labelId }
            };

            var recordsDTO = records.Select(r => new RecordDTO { Id = r.Id, Name = r.Name }).ToList();

            _unitOfWorkMock.SetupUnitOfWorkLabelGetByIdAsync(label, cancellationToken);
            _mapperMock.SetupMap(model, label);

            _validatorMock.SetupValidatorMock(new ValidationResult(), cancellationToken);
            _unitOfWorkMock.SetupUnitOfWorkLabelUpdateAsync(cancellationToken);
            _unitOfWorkMock.SetupGetLabelsRecordsAsync(records, cancellationToken);

            _mapperMock.Setup(m => m.Map<IEnumerable<RecordDTO>>(records)).Returns(recordsDTO);

            _elasticClientMock.SetupIndexManyAsync(recordsDTO);

            // Act
            await _labelCatalogService.UpdateAsync(labelId, model, cancellationToken);

            // Assert
            _unitOfWorkMock.Verify();
            _validatorMock.Verify();
            _mapperMock.Verify(m => m.Map(model, label), Times.Once);
            _elasticClientMock.Verify();
        }

        [Fact]
        public async Task UpdateAsync_LabelNotFound_ThrowsArgumentException()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var labelId = Guid.NewGuid();

            _unitOfWorkMock.SetupUnitOfWorkLabelGetByIdAsync(null, cancellationToken);

            // Act
            Func<Task> updateAsyncTask = async () => await _labelCatalogService.UpdateAsync(labelId, new LabelModel(), cancellationToken);

            // Assert
            await updateAsyncTask
                .Should()
                .ThrowAsync<ArgumentException>();
        }
    }
}