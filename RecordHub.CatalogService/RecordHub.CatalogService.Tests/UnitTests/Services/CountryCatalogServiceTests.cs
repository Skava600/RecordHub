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
    public class CountryCatalogServiceTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IElasticClient> _elasticClientMock;
        private readonly Mock<IValidator<BaseEntity>> _validatorMock;
        private readonly CountryCatalogService _countryCatalogService;
        private readonly BaseEntityGenerator _baseEntityGenerator;

        public CountryCatalogServiceTests()
        {
            _baseEntityGenerator = new BaseEntityGenerator();
            _mapperMock = new Mock<IMapper>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _validatorMock = new Mock<IValidator<BaseEntity>>();
            _elasticClientMock = new Mock<IElasticClient>();
            _countryCatalogService = new CountryCatalogService(
                _mapperMock.Object,
                _unitOfWorkMock.Object,
                _validatorMock.Object,
                _elasticClientMock.Object);
        }

        [Fact]
        public async Task AddAsync_ValidModel_AddsCountryAndCommitsUnitOfWork()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var model = _baseEntityGenerator.GenerateCountryModel();

            var country = new Country
            {
                Id = Guid.NewGuid(),
                Name = model.Name,
                Slug = model.Slug,
            };

            _mapperMock.SetupMap(model, country);
            var validResult = new ValidationResult();
            _validatorMock.SetupValidatorMock(validResult, cancellationToken);

            _unitOfWorkMock.SetupUnitOfWorkMockAddCountryAsync(cancellationToken);

            // Act
            await _countryCatalogService.AddAsync(model, cancellationToken);

            // Assert
            _validatorMock.Verify(); // Verify that ValidateAndThrowAsync was called

            _unitOfWorkMock
                .Verify(uow => uow.Countries
                .AddAsync(country, cancellationToken),
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
            var model = _baseEntityGenerator.GenerateCountryModel();

            var country = new Country
            {
                Id = Guid.NewGuid(),
                Name = model.Name,
                Slug = model.Slug
            };

            _mapperMock.SetupMap(model, country);

            var validationException = new ValidationException("Validation error");
            _validatorMock.SetupValidatorMockThrowsException(validationException);

            // Act and Assert
            await _countryCatalogService
                .Invoking(async service => await service
                .AddAsync(model, cancellationToken))
                .Should()
                .ThrowAsync<ValidationException>()
                .WithMessage(validationException.Message);

            _unitOfWorkMock
                .Verify(uow => uow.Countries
                .AddAsync(It.IsAny<Country>(), cancellationToken),
                Times.Never); // Verify AddAsync was not called

            _unitOfWorkMock
                .Verify(uow => uow
                .CommitAsync(cancellationToken),
                Times.Never); // Verify CommitAsync was not called
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllCountries()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var entities = _baseEntityGenerator.GenerateBaseEntities();

            var countries = entities.Select(e => new Country
            {
                Id = e.Id,
                Name = e.Name,
                Slug = e.Slug
            });

            var expectedCountryDTOs = countries.Select(c => new CountryDTO
            {
                Name = c.Name,
                Slug = c.Slug
            });

            _unitOfWorkMock.SetupUnitOfWorkMockGetAllCountriesAsync(countries, cancellationToken);
            _mapperMock.SetupMap(countries, expectedCountryDTOs);

            // Act
            var result = await _countryCatalogService.GetAllAsync(cancellationToken);

            // Assert
            result
                .Should()
                .BeEquivalentTo(expectedCountryDTOs);
        }

        [Fact]
        public async Task DeleteAsync_CountryFound_DeletesCountryAndIndexesInElasticsearch()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;

            var baseEntity = _baseEntityGenerator.GenerateBaseEntity();
            var country = new Country
            {
                Id = baseEntity.Id,
                Name = baseEntity.Name,
                Slug = baseEntity.Slug
            };

            _unitOfWorkMock.SetupUnitOfWorkMockCountryDeleteAsync(country, cancellationToken);
            _elasticClientMock.SetupDeleteByQueryAsync(It.IsAny<Func<QueryContainerDescriptor<RecordDTO>, QueryContainer>>());

            // Act
            await _countryCatalogService.DeleteAsync(country.Id, cancellationToken);

            // Assert
            _unitOfWorkMock.Verify();
            _elasticClientMock.Verify();
        }

        [Fact]
        public async Task UpdateAsync_CountryFound_UpdatesCountryAndIndexesRecords()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var countryId = Guid.NewGuid();

            var model = _baseEntityGenerator.GenerateCountryModel();

            var country = new Country
            {
                Id = countryId,
                Name = model.Name,
                Slug = model.Slug
            };

            var records = new List<Record>
            {
                new Record { Id = Guid.NewGuid(), Name = "Record 1", CountryId = countryId },
                new Record { Id = Guid.NewGuid(), Name = "Record 2", CountryId = countryId }
            };

            var recordsDTO = records.Select(r => new RecordDTO { Id = r.Id, Name = r.Name }).ToList();

            _unitOfWorkMock.SetupUnitOfWorkMockCountryGetByIdAsync(country, cancellationToken);
            _mapperMock.SetupMap(model, country);

            _validatorMock.SetupValidatorMock(new ValidationResult(), cancellationToken);
            _unitOfWorkMock.SetupUnitOfWorkCountryUpdateAsync(cancellationToken);
            _unitOfWorkMock.SetupGetCountriesRecordsAsync(records, cancellationToken);

            _mapperMock.Setup(m => m.Map<IEnumerable<RecordDTO>>(records)).Returns(recordsDTO);

            _elasticClientMock.SetupIndexManyAsync(recordsDTO);

            // Act
            await _countryCatalogService.UpdateAsync(countryId, model, cancellationToken);

            // Assert
            _unitOfWorkMock.Verify();
            _validatorMock.Verify();
            _mapperMock.Verify(m => m.Map(model, country), Times.Once);
            _elasticClientMock.Verify();
        }

        [Fact]
        public async Task UpdateAsync_CountryNotFound_ThrowsArgumentException()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;
            var countryId = Guid.NewGuid();

            _unitOfWorkMock.SetupUnitOfWorkMockCountryGetByIdAsync(null, cancellationToken);

            // Act
            Func<Task> updateAsyncTask = async () => await _countryCatalogService.UpdateAsync(countryId, new CountryModel(), cancellationToken);

            // Assert
            await updateAsyncTask
                .Should()
                .ThrowAsync<ArgumentException>();
        }
    }
}
