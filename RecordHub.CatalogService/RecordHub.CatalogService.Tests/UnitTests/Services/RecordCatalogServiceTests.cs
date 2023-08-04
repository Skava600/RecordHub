using AutoMapper;
using FluentAssertions;
using FluentValidation;
using Microsoft.Extensions.Options;
using Nest;
using RecordHub.CatalogService.Application.Data;
using RecordHub.CatalogService.Application.DTO;
using RecordHub.CatalogService.Application.Services;
using RecordHub.CatalogService.Domain.Entities;
using RecordHub.CatalogService.Domain.Models;
using RecordHub.CatalogService.Infrastructure.Config;
using RecordHub.CatalogService.Infrastructure.Services;
using RecordHub.CatalogService.Tests.Generators;
using RecordHub.CatalogService.Tests.Setups;
using RecordHub.Shared.Exceptions;
using ValidationException = FluentValidation.ValidationException;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace RecordHub.CatalogService.Tests.UnitTests.Services;

public class RecordCatalogServiceTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IValidator<Domain.Entities.Record>> _validatorMock;
    private readonly Mock<IElasticClient> _elasticClientMock;
    private readonly Mock<IRedisCacheService> _cacheServiceMock;
    private readonly Mock<IOptions<ElasticsearchConfig>> _elasticsearchConfigMock;
    private readonly RecordCatalogService _recordCatalogService;
    private readonly RecordGenerator recordGenerator;

    public RecordCatalogServiceTests()
    {
        recordGenerator = new RecordGenerator();
        _mapperMock = new Mock<IMapper>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _validatorMock = new Mock<IValidator<Domain.Entities.Record>>();
        _elasticClientMock = new Mock<IElasticClient>();
        _elasticsearchConfigMock = new Mock<IOptions<ElasticsearchConfig>>();
        _cacheServiceMock = new Mock<IRedisCacheService>();

        _recordCatalogService = new RecordCatalogService(
            _mapperMock.Object,
            _unitOfWorkMock.Object,
            _validatorMock.Object,
            _elasticClientMock.Object,
            _elasticsearchConfigMock.Object,
            _cacheServiceMock.Object);
    }

    [Fact]
    public async Task AddAsync_ValidModel_RecordAddedAndIndexedInElasticsearch()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var model = recordGenerator.GenerateRecordModel();
        var record = new Domain.Entities.Record
        {
            Name = model.Name,
            Radius = model.Radius ?? 0,
            Year = model.Year ?? 0,
            Description = model.Description,
            Price = model.Price ?? 0,
            Label = new Label { Name = model.Label },
            Country = new Country { Name = model.Country },
            Artist = new Artist { Name = model.Artist },
            Styles = model.Styles.Select(style => new Style { Name = style }).ToList()
        };

        var recordDTO = new RecordDTO
        {
            Id = Guid.NewGuid(),
            Name = model.Name,
            Radius = model.Radius ?? 0,
            Year = model.Year ?? 0,
            Description = model.Description,
            Price = model.Price ?? 0,
            Label = new LabelDTO { Name = model.Label },
            Country = new CountryDTO { Name = model.Country },
            Artist = new ArtistDTO { Name = model.Artist },
            Styles = model.Styles.Select(style => new StyleDTO { Name = style }).ToList()
        };

        var validResult = new ValidationResult();
        _validatorMock.SetupValidatorMock(validResult);

        _mapperMock.SetupMap(model, record);
        _mapperMock.SetupMap(record, recordDTO);

        _unitOfWorkMock.SetupUnitOfWorkMockAddRecordAsync(cancellationToken);

        _elasticClientMock.SetupElasticClientMock();

        // Act
        await _recordCatalogService.AddAsync(model, cancellationToken);

        // Assert
        _validatorMock.Verify();
        _mapperMock.Verify();
        _unitOfWorkMock.Verify(); // AddAsync and CommitAsync were called
        _elasticClientMock.Verify(); // IndexAsync was called
    }

    [Fact]
    public async Task AddAsync_InvalidModel_ThrowsValidationException()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var model = new RecordModel
        {
            Name = "",
            Price = 0.0
        };

        var record = new Domain.Entities.Record
        {
            Name = model.Name,
            Price = model.Price ?? 0,
        };

        _mapperMock.SetupMap(model, record);

        var validationException = new ValidationException("Validation failed");
        _validatorMock.SetupValidatorMockThrowsException(validationException);

        // Act and Assert
        await _recordCatalogService
            .Invoking(async service => await service
            .AddAsync(model, cancellationToken))
            .Should()
            .ThrowAsync<ValidationException>();

        _unitOfWorkMock
            .Verify(uow => uow.Records
            .AddAsync(It.IsAny<Domain.Entities.Record>(), cancellationToken),
            Times.Never); // Verify AddAsync was not called

        _unitOfWorkMock
            .Verify(uow => uow
            .CommitAsync(cancellationToken),
            Times.Never);

        _elasticClientMock
            .Verify(ec => ec
            .IndexAsync(
                It.IsAny<RecordDTO>(),
                It.IsAny<Func<IndexDescriptor<RecordDTO>, IIndexRequest<RecordDTO>>>(),
                cancellationToken),
            Times.Never); // Verify IndexAsync was not called
    }

    [Fact]
    public async Task DeleteAsync_RecordExists_DeletesRecordAndIndexesInElasticsearch()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var recordId = Guid.NewGuid();

        var deletedRecord = new Domain.Entities.Record
        {
            Id = recordId
        };

        _unitOfWorkMock.SetupUnitOfWorkMockRecordDeleteAsync(deletedRecord, cancellationToken);

        // Act
        await _recordCatalogService.DeleteAsync(recordId, cancellationToken);

        // Assert
        _unitOfWorkMock.Verify();

        _elasticClientMock
            .Verify(ec => ec
            .DeleteAsync(
                recordId,
                It.IsAny<Func<DeleteDescriptor<RecordDTO>, IDeleteRequest>>(),
                cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_RecordDoesNotExist_DoesNotDeleteOrIndexInElasticsearch()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var recordId = Guid.NewGuid();

        _unitOfWorkMock.SetupUnitOfWorkMockRecordDeleteAsync(null);

        // Act
        await _recordCatalogService.DeleteAsync(recordId, cancellationToken);

        // Assert
        _unitOfWorkMock
            .Verify(repo => repo.Records
            .DeleteAsync(recordId, cancellationToken),
            Times.Once);

        _unitOfWorkMock
            .Verify(repo => repo
            .CommitAsync(cancellationToken),
            Times.Never);

        _elasticClientMock.Verify(ec => ec
            .DeleteAsync(
                recordId,
                It.IsAny<Func<DeleteDescriptor<RecordDTO>, IDeleteRequest>>(),
                cancellationToken),
            Times.Never);
    }

    [Fact]
    public async Task GetBySlugAsync_RecordFoundInRepository_ReturnsRecordDTO()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var slug = "sample-slug";

        var expectedRecord = recordGenerator.GenerateRecord();

        _cacheServiceMock.SetupCacheServiceForGetAsync((RecordDTO?)null);
        _unitOfWorkMock.SetupUnitOfWorkMockRecordGetBySlugAsync(expectedRecord, cancellationToken);

        var expectedRecordDTO = new RecordDTO
        {
            Id = expectedRecord.Id,
            Name = expectedRecord.Name,
            Slug = expectedRecord.Slug,
            Radius = expectedRecord.Radius,
            Year = expectedRecord.Year,
            Description = expectedRecord.Description,
            Price = expectedRecord.Price,
            Label = new LabelDTO { Name = expectedRecord.Label.Name, Slug = expectedRecord.Label.Slug },
            Country = new CountryDTO { Name = expectedRecord.Country.Name, Slug = expectedRecord.Country.Slug },
            Artist = new ArtistDTO { Name = expectedRecord.Artist.Name, Slug = expectedRecord.Artist.Slug },
            Styles = expectedRecord.Styles.Select(s => new StyleDTO { Name = s.Name, Slug = s.Slug }).ToList()
        };

        _mapperMock.SetupMap(expectedRecord, expectedRecordDTO);

        // Act
        var result = await _recordCatalogService.GetBySlugAsync(slug, cancellationToken);

        // Assert
        result
            .Should()
            .BeEquivalentTo(expectedRecordDTO);
    }

    [Fact]
    public async Task GetByPageAsync_CachedValueExists_ReturnsCachedData()
    {
        // Arrange

        var expectedData = new List<RecordDTO>
        {
            recordGenerator.GenerateRecordDTO(),
            recordGenerator.GenerateRecordDTO()
        };

        _cacheServiceMock.SetupCacheServiceForGetAsync<IEnumerable<RecordDTO>>(expectedData);

        // Act
        var result = await _recordCatalogService.GetByPageAsync(1, 10);

        // Assert
        result
            .Should()
            .BeEquivalentTo(expectedData);

        _cacheServiceMock
            .Verify(cache => cache
            .GetAsync<IEnumerable<RecordDTO>>(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }


    [Fact]
    public async Task GetBySlugAsync_RecordNotFound_ThrowsEntityNotFoundException()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var slug = "non-existing-slug";

        _cacheServiceMock.SetupCacheServiceForGetAsync((RecordDTO?)null);
        _unitOfWorkMock.SetupUnitOfWorkMockRecordGetBySlugAsync(null, cancellationToken);

        // Act and Assert
        await _recordCatalogService
            .Invoking(async service => await service
            .GetBySlugAsync(slug, cancellationToken))
            .Should().ThrowAsync<EntityNotFoundException>();
    }

    [Theory]
    [InlineData(1, 10, 0, 100, 2000, 2023, new short[] { 10, 15 }, new string[] { "style1", "style2" }, new string[] { "artist1", "artist2" }, new string[] { "country1", "country2" }, new string[] { "label1", "label2" })]
    public async Task GetByPageAsync_ValidFilterModel_ReturnsRecords(int page, int pageSize, int minPrice, int maxPrice, int minYear, int maxYear,
       short[] radiuses, string[] styles, string[] artists, string[] countries, string[] labels)
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var filterModel = new RecordFilterModel
        {
            Page = page,
            PageSize = pageSize,
            MinPrice = minPrice,
            MaxPrice = maxPrice,
            MinYear = minYear,
            MaxYear = maxYear,
            Radiuses = radiuses,
            Styles = styles,
            Artists = artists,
            Countries = countries,
            Labels = labels
        };

        var expectedRecords = new List<RecordDTO>
        {
            recordGenerator.GenerateRecordDTO(),
            recordGenerator.GenerateRecordDTO(),
        };

        _cacheServiceMock.SetupCacheServiceForGetAsync<IEnumerable<RecordDTO>>((IEnumerable<RecordDTO>?)null);
        _elasticClientMock.SetupSearchAsync(expectedRecords.AsReadOnly(), cancellationToken);

        // Act
        var result = await _recordCatalogService.GetByPageAsync(filterModel, cancellationToken);

        // Assert
        result
            .Should()
            .BeEquivalentTo(expectedRecords);
    }

    [Fact]
    public async Task UpdateAsync_ValidRecord_SuccessfullyUpdatesRecord()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var recordId = Guid.NewGuid();

        var model = recordGenerator.GenerateRecordModel();

        var record = new Domain.Entities.Record
        {
            Name = model.Name,
            Radius = model.Radius ?? 0,
            Year = model.Year ?? 0,
            Description = model.Description,
            Price = model.Price ?? 0,
            Label = new Label { Name = model.Label },
            Country = new Country { Name = model.Country },
            Artist = new Artist { Name = model.Artist },
            Styles = model.Styles.Select(style => new Style { Name = style }).ToList()
        };

        var recordDTO = new RecordDTO
        {
            Id = Guid.NewGuid(),
            Name = model.Name,
            Radius = model.Radius ?? 0,
            Year = model.Year ?? 0,
            Description = model.Description,
            Price = model.Price ?? 0,
            Label = new LabelDTO { Name = model.Label },
            Country = new CountryDTO { Name = model.Country },
            Artist = new ArtistDTO { Name = model.Artist },
            Styles = model.Styles.Select(style => new StyleDTO { Name = style }).ToList()
        };

        _unitOfWorkMock.SetupGetByIdRecordGraphIncludedAsync(record, cancellationToken);

        _mapperMock.SetupMap(model, record);

        var validResult = new ValidationResult();
        _validatorMock.SetupValidatorMock(validResult, cancellationToken);

        _unitOfWorkMock.SetupUnitOfWorkRecordUpdateAsync(cancellationToken);

        var updateResponseMock = new Mock<UpdateResponse<RecordDTO>>();
        _elasticClientMock.SetupUpdateAsync(updateResponseMock.Object, cancellationToken);

        _mapperMock.SetupMap(record, recordDTO);

        // Act
        await _recordCatalogService.UpdateAsync(recordId, model, cancellationToken);

        // Assert
        _validatorMock.Verify();
        _unitOfWorkMock.Verify();
        _unitOfWorkMock.Verify();
        _elasticClientMock.Verify();
    }

    [Fact]
    public async Task UpdateAsync_RecordNotFound_ThrowsArgumentException()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var recordId = Guid.NewGuid();

        var model = new RecordModel
        {
        };

        _unitOfWorkMock.SetupGetByIdRecordGraphIncludedAsync(null, cancellationToken); // Return null to simulate record not found

        // Act
        Func<Task> act = async () => await _recordCatalogService.UpdateAsync(recordId, model, cancellationToken);

        // Assert
        await act
            .Should()
            .ThrowAsync<ArgumentException>();

        _unitOfWorkMock
            .Verify(repo => repo.Records
            .UpdateAsync(It.IsAny<Domain.Entities.Record>(), cancellationToken),
            Times.Never); // Verify UpdateAsync was not called

        _unitOfWorkMock
            .Verify(repo => repo
            .CommitAsync(cancellationToken),
            Times.Never); // Verify CommitAsync was not called

        _elasticClientMock
            .Verify(client => client
            .UpdateAsync<RecordDTO>(
                recordId,
                It.IsAny<Func<UpdateDescriptor<RecordDTO, RecordDTO>, IUpdateRequest<RecordDTO, RecordDTO>>>(),
                It.IsAny<CancellationToken>()),
            Times.Never); // Verify UpdateAsync on elastic client was not called
    }

    [Fact]
    public async Task SearchAsync_ValidSearchResponse_ReturnsListOfRecordDTO()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var searchText = "sample search text";

        var expectedRecords = new List<RecordDTO>
        {
           recordGenerator.GenerateRecordDTO(),
           recordGenerator.GenerateRecordDTO()
        };

        _elasticClientMock.SetupSearchAsync(expectedRecords.AsReadOnly(), cancellationToken);
        _cacheServiceMock.SetupCacheServiceForGetAsync<IEnumerable<RecordDTO>>((IEnumerable<RecordDTO>?)null);

        // Act
        var result = await _recordCatalogService.SearchAsync(searchText, cancellationToken);

        // Assert
        result
            .Should()
            .BeEquivalentTo(expectedRecords);
    }

    [Fact]
    public async Task SearchAsync_CachedValueExists_ReturnsCachedData()
    {
        // Arrange
        var searchText = "sample search text";
        var expectedRecords = new List<RecordDTO>
        {
           recordGenerator.GenerateRecordDTO(),
           recordGenerator.GenerateRecordDTO()
        };

        _cacheServiceMock.SetupCacheServiceForGetAsync<IEnumerable<RecordDTO>>(expectedRecords);


        // Act
        var result = await _recordCatalogService.SearchAsync(searchText);

        // Assert
        result
            .Should()
            .BeEquivalentTo(expectedRecords);

        _cacheServiceMock
            .Verify(cache => cache
            .GetAsync<IEnumerable<RecordDTO>>(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task SearchAsync_InvalidSearchResponse_ThrowsOriginalException()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var searchText = "invalid search text";

        var originalException = new Exception("Search failed");

        _cacheServiceMock.SetupCacheServiceForGetAsync<IEnumerable<RecordDTO>>((IEnumerable<RecordDTO>?)null);
        _elasticClientMock.SetupInvalidSearchAsync<RecordDTO>(originalException, cancellationToken);

        // Act and Assert
        await _recordCatalogService.Invoking(async service => await service.SearchAsync(searchText, cancellationToken))
                   .Should().ThrowAsync<Exception>().WithMessage(originalException.Message);
    }
}