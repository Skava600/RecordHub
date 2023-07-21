using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Options;
using Nest;
using RecordHub.CatalogService.Application.Data;
using RecordHub.CatalogService.Application.DTO;
using RecordHub.CatalogService.Domain.Entities;
using RecordHub.CatalogService.Domain.Models;
using RecordHub.CatalogService.Infrastructure.Config;
using RecordHub.CatalogService.Infrastructure.Services;
using RecordHub.CatalogService.Tests.Generators;
using RecordHub.Shared.Exceptions;
using ValidationException = FluentValidation.ValidationException;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace RecordHub.CatalogService.Tests;

public class RecordCatalogServiceTests
{
    private Mock<IMapper> _mapperMock;
    private Mock<IUnitOfWork> _unitOfWorkMock;
    private Mock<IValidator<Domain.Entities.Record>> _validatorMock;
    private Mock<IElasticClient> _elasticClientMock;
    private Mock<IOptions<ElasticsearchConfig>> _elasticsearchConfigMock;
    private RecordCatalogService _recordCatalogService;
    private RecordGenerator recordGenerator;

    public RecordCatalogServiceTests()
    {
        recordGenerator = new RecordGenerator();
        _mapperMock = new Mock<IMapper>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _validatorMock = new Mock<IValidator<Domain.Entities.Record>>();
        _elasticClientMock = new Mock<IElasticClient>();
        _elasticsearchConfigMock = new Mock<IOptions<ElasticsearchConfig>>();
        _recordCatalogService = new RecordCatalogService(
            _mapperMock.Object,
            _unitOfWorkMock.Object,
            _validatorMock.Object,
            _elasticClientMock.Object,
            _elasticsearchConfigMock.Object
        );
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
        _validatorMock
           .Setup(_ => _.ValidateAsync(It.IsAny<IValidationContext>(), cancellationToken))
           .ReturnsAsync(validResult)
           .Verifiable();

        _mapperMock
            .Setup(m => m.Map<Domain.Entities.Record>(model))
            .Returns(record);

        _mapperMock
            .Setup(m => m
            .Map<RecordDTO>(record))
            .Returns(recordDTO);

        _unitOfWorkMock
            .Setup(uow => uow
            .Records.AddAsync(record, cancellationToken))
            .Verifiable();

        _unitOfWorkMock
            .Setup(uow => uow
            .CommitAsync(cancellationToken))
            .Verifiable();

        _elasticClientMock
            .Setup(ec => ec
            .IndexAsync(
                recordDTO,
                It.IsAny<Func<IndexDescriptor<RecordDTO>,
                IIndexRequest<RecordDTO>>>(),
                cancellationToken))
            .Verifiable();

        // Act
        await _recordCatalogService.AddAsync(model, cancellationToken);

        // Assert
        _validatorMock.Verify();
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

        _mapperMock
            .Setup(m => m.Map<Domain.Entities.Record>(model))
            .Returns(record);

        _validatorMock
            .Setup(v => v
            .ValidateAsync(It.IsAny<IValidationContext>(), cancellationToken))
            .Throws(new ValidationException("Validation failed"));

        // Act and Assert
        await Assert.ThrowsAsync<ValidationException>(() => _recordCatalogService.AddAsync(model, cancellationToken));
        _unitOfWorkMock.Verify(uow => uow.Records.AddAsync(It.IsAny<Domain.Entities.Record>(), cancellationToken), Times.Never); // Verify AddAsync was not called
        _unitOfWorkMock.Verify(uow => uow.CommitAsync(cancellationToken), Times.Never);
        _elasticClientMock.Verify(ec => ec.IndexAsync(It.IsAny<RecordDTO>(), It.IsAny<Func<IndexDescriptor<RecordDTO>, IIndexRequest<RecordDTO>>>(), cancellationToken), Times.Never); // Verify IndexAsync was not called
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

        _unitOfWorkMock.Setup(repo => repo.Records.DeleteAsync(recordId, cancellationToken)).ReturnsAsync(deletedRecord);

        // Act
        await _recordCatalogService.DeleteAsync(recordId, cancellationToken);

        // Assert
        _unitOfWorkMock.Verify(repo => repo.Records.DeleteAsync(recordId, cancellationToken), Times.Once);
        _unitOfWorkMock.Verify(repo => repo.CommitAsync(cancellationToken), Times.Once);
        _elasticClientMock.Verify(ec => ec.DeleteAsync<RecordDTO>(recordId, It.IsAny<Func<DeleteDescriptor<RecordDTO>, IDeleteRequest>>(), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_RecordDoesNotExist_DoesNotDeleteOrIndexInElasticsearch()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var recordId = Guid.NewGuid();

        _unitOfWorkMock.Setup(repo => repo.Records.DeleteAsync(recordId, cancellationToken)).ReturnsAsync((Domain.Entities.Record)null);

        // Act
        await _recordCatalogService.DeleteAsync(recordId, cancellationToken);

        // Assert
        _unitOfWorkMock.Verify(repo => repo.Records.DeleteAsync(recordId, cancellationToken), Times.Once);
        _unitOfWorkMock.Verify(repo => repo.CommitAsync(cancellationToken), Times.Never);
        _elasticClientMock.Verify(ec => ec.DeleteAsync<RecordDTO>(recordId, It.IsAny<Func<DeleteDescriptor<RecordDTO>, IDeleteRequest>>(), cancellationToken), Times.Never);
    }

    [Fact]
    public async Task GetByIdAsync_RecordFoundInElasticsearch_ReturnsRecordDTO()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var recordId = Guid.NewGuid();


        var expectedRecord = recordGenerator.GenerateRecordDTO();
        var getResponseMock = new Mock<GetResponse<RecordDTO>>();
        getResponseMock.SetReturnsDefault<bool>(true);

        _elasticClientMock
            .Setup(ec => ec.GetAsync<RecordDTO>(recordId, It.IsAny<Func<GetDescriptor<RecordDTO>, IGetRequest>>(), cancellationToken))
            .ReturnsAsync(getResponseMock.Object);

        // Act
        var result = await _recordCatalogService.GetByIdAsync(recordId, cancellationToken);

        // Assert
        Assert.Equal(expectedRecord, result);
    }

    [Fact]
    public async Task GetBySlugAsync_RecordFound_ReturnsRecordDTO()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var slug = "sample-slug";

        var expectedRecord = recordGenerator.GenerateRecord();

        _unitOfWorkMock
            .Setup(repo => repo.Records.GetBySlugAsync(slug, cancellationToken))
            .ReturnsAsync(expectedRecord);

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

        _mapperMock
            .Setup(mapper => mapper.Map<RecordDTO>(expectedRecord))
            .Returns(expectedRecordDTO);

        // Act
        var result = await _recordCatalogService.GetBySlugAsync(slug, cancellationToken);

        // Assert
        Assert.Equal(expectedRecordDTO, result);
    }

    [Fact]
    public async Task GetBySlugAsync_RecordNotFound_ThrowsEntityNotFoundException()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var slug = "non-existing-slug";

        _unitOfWorkMock
            .Setup(repo => repo.Records.GetBySlugAsync(slug, cancellationToken))
            .ReturnsAsync((Domain.Entities.Record)null);

        // Act and Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _recordCatalogService.GetBySlugAsync(slug, cancellationToken));
    }

    [Fact]
    public async Task GetByPageAsync_ValidFilterModel_ReturnsRecords()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var filterModel = new RecordFilterModel
        {
            Page = 1,
            PageSize = 10,
            MinPrice = 0,
            MaxPrice = 100,
            MinYear = 2000,
            MaxYear = 2023,
            Radiuses = new List<short> { 10, 15 },
            Styles = new List<string> { "style1", "style2" },
            Artists = new List<string> { "artist1", "artist2" },
            Countries = new List<string> { "country1", "country2" },
            Labels = new List<string> { "label1", "label2" }
        };

        var expectedRecords = new List<RecordDTO>
        {
            // Set up expected record DTOs based on the filterModel properties
        };

        var searchResponseMock = new Mock<ISearchResponse<RecordDTO>>();
        searchResponseMock.Setup(r => r.Documents).Returns(expectedRecords);

        _elasticClientMock
            .Setup(ec => ec.SearchAsync<RecordDTO>(It.IsAny<Func<SearchDescriptor<RecordDTO>, ISearchRequest>>(), cancellationToken))
            .ReturnsAsync(searchResponseMock.Object);

        // Act
        var result = await _recordCatalogService.GetByPageAsync(filterModel, cancellationToken);

        // Assert
        Assert.Equal(expectedRecords, result);
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

        _unitOfWorkMock.Setup(repo => repo.Records.GetByIdGraphIncludedAsync(recordId, cancellationToken))
                       .ReturnsAsync(record);

        _mapperMock.Setup(mapper => mapper.Map(model, record)).Returns(record);

        var validResult = new ValidationResult();
        _validatorMock
           .Setup(_ => _.ValidateAsync(It.IsAny<IValidationContext>(), cancellationToken))
           .ReturnsAsync(validResult)
           .Verifiable();

        _unitOfWorkMock.Setup(repo => repo.Records.UpdateAsync(record, cancellationToken)).Returns(Task.CompletedTask);

        _unitOfWorkMock.Setup(repo => repo.CommitAsync(cancellationToken)).Returns(Task.CompletedTask);

        var updateResponseMock = new Mock<UpdateResponse<RecordDTO>>();
        _elasticClientMock.Setup(client => client.UpdateAsync<RecordDTO>(
                                       recordId,
                                       It.IsAny<Func<UpdateDescriptor<RecordDTO, RecordDTO>, IUpdateRequest<RecordDTO, RecordDTO>>>(),
                                       It.IsAny<CancellationToken>()))
                          .ReturnsAsync(updateResponseMock.Object);

        _mapperMock.Setup(mapper => mapper.Map<RecordDTO>(record)).Returns(recordDTO);

        // Act
        await _recordCatalogService.UpdateAsync(recordId, model, cancellationToken);

        // Assert
        _validatorMock.Verify();
        _unitOfWorkMock.Verify(repo => repo.Records.UpdateAsync(record, cancellationToken), Times.Once);
        _unitOfWorkMock.Verify(repo => repo.CommitAsync(cancellationToken), Times.Once);
        _elasticClientMock
            .Verify(client => client
            .UpdateAsync<RecordDTO>(
                recordDTO.Id,
                It.IsAny<Func<UpdateDescriptor<RecordDTO, RecordDTO>, IUpdateRequest<RecordDTO, RecordDTO>>>(), cancellationToken),
             Times.Once);
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

        _unitOfWorkMock.Setup(repo => repo.Records.GetByIdGraphIncludedAsync(recordId, cancellationToken))
                       .ReturnsAsync((Domain.Entities.Record)null); // Return null to simulate record not found

        // Act and Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _recordCatalogService.UpdateAsync(recordId, model, cancellationToken));
        _unitOfWorkMock.Verify(repo => repo.Records.UpdateAsync(It.IsAny<Domain.Entities.Record>(), cancellationToken), Times.Never); // Verify UpdateAsync was not called
        _unitOfWorkMock.Verify(repo => repo.CommitAsync(cancellationToken), Times.Never); // Verify CommitAsync was not called
        _elasticClientMock.Verify(client => client.UpdateAsync<RecordDTO>(
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

        var searchResponseMock = new Mock<ISearchResponse<RecordDTO>>();
        searchResponseMock.Setup(r => r.IsValid).Returns(true);
        searchResponseMock.Setup(r => r.Documents).Returns(expectedRecords);

        _elasticClientMock.Setup(client => client.SearchAsync<RecordDTO>(
            It.IsAny<Func<SearchDescriptor<RecordDTO>, ISearchRequest>>(), cancellationToken))
            .ReturnsAsync(searchResponseMock.Object);

        // Act
        var result = await _recordCatalogService.SearchAsync(searchText, cancellationToken);

        // Assert
        Assert.Equal(expectedRecords, result);
    }

    [Fact]
    public async Task SearchAsync_InvalidSearchResponse_ThrowsOriginalException()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var searchText = "invalid search text";

        var originalException = new Exception("Search failed");

        var searchResponseMock = new Mock<ISearchResponse<RecordDTO>>();
        searchResponseMock.Setup(r => r.IsValid).Returns(false);
        searchResponseMock.Setup(r => r.OriginalException).Returns(originalException);

        _elasticClientMock.Setup(client => client.SearchAsync<RecordDTO>(
            It.IsAny<Func<SearchDescriptor<RecordDTO>, ISearchRequest>>(), cancellationToken))
            .ReturnsAsync(searchResponseMock.Object);

        // Act and Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _recordCatalogService.SearchAsync(searchText, cancellationToken));
        Assert.Equal(originalException, exception);
    }
}