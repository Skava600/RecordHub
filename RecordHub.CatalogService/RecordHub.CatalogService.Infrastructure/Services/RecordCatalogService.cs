using AutoMapper;
using FluentValidation;
using MassTransit.Internals;
using Microsoft.Extensions.Options;
using Nest;
using RecordHub.CatalogService.Application.Data;
using RecordHub.CatalogService.Application.DTO;
using RecordHub.CatalogService.Application.Services;
using RecordHub.CatalogService.Domain.Entities;
using RecordHub.CatalogService.Domain.Models;
using RecordHub.CatalogService.Infrastructure.Config;
using RecordHub.Shared.Exceptions;

namespace RecordHub.CatalogService.Infrastructure.Services
{
    public class RecordCatalogService : IRecordCatalogService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _repository;
        private readonly IValidator<Record> _validator;
        private readonly IElasticClient _elasticClient;
        private readonly ElasticsearchConfig _elasticsearchConfig;

        public RecordCatalogService(IMapper mapper,
            IUnitOfWork repository,
            IValidator<Record> validator,
            IElasticClient elasticClient,
            IOptions<ElasticsearchConfig> elasticsearchConfig)
        {
            _mapper = mapper;
            _repository = repository;
            _validator = validator;
            _elasticClient = elasticClient;
            _elasticsearchConfig = elasticsearchConfig.Value;
        }

        public async Task AddAsync(RecordModel model, CancellationToken cancellationToken = default)
        {
            var record = _mapper.Map<Record>(model);

            await _validator.ValidateAndThrowAsync(record, cancellationToken);

            await _repository.Records.AddAsync(record, cancellationToken);
            await _repository.CommitAsync();

            var recordDTO = _mapper.Map<RecordDTO>(record);
            await _elasticClient.IndexAsync(recordDTO, i => i.Id(recordDTO.Id), cancellationToken);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var deletedRecord = await _repository.Records.DeleteAsync(id, cancellationToken);
            if (deletedRecord != null)
            {
                await _repository.CommitAsync();

                await _elasticClient.DeleteAsync<RecordDTO>(deletedRecord.Id);
            }
        }

        public async Task<RecordDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var response = await _elasticClient.GetAsync<RecordDTO>(id, ct: cancellationToken);
            if (response.IsValid && response.Found)
            {
                return response.Source;
            }

            var record = await _repository.Records.GetByIdAsync(id, cancellationToken);
            return _mapper.Map<RecordDTO>(record);
        }

        public async Task<IEnumerable<RecordDTO>> GetByPageAsync(
            int page,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            IEnumerable<Record> records = await _repository.Records.GetByPageAsync(page, pageSize, cancellationToken);
            IEnumerable<RecordDTO> result = _mapper.Map<IEnumerable<Record>, IEnumerable<RecordDTO>>(records);
            return result;
        }

        public async Task<RecordDTO?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
        {
            Record? record = await _repository.Records.GetBySlugAsync(slug, cancellationToken);

            if (record == null)
            {
                throw new EntityNotFoundException(nameof(slug));
            }

            var result = _mapper.Map<RecordDTO>(record);

            return result;
        }

        public Task<int> GetCountAsync(CancellationToken cancellationToken = default)
        {
            return _repository.Records.GetCountAsync(cancellationToken);
        }

        public async Task<IEnumerable<RecordDTO>> GetByPageAsync(RecordFilterModel filterModel, CancellationToken cancellationToken = default)
        {
            var searchResponse = await _elasticClient.SearchAsync<RecordDTO>(s => s
            .From((filterModel.Page - 1) * filterModel.PageSize)
            .Size(filterModel.PageSize)
             .Query(q => q
                 .Bool(b => b
                     .Must(
                         // Price range query
                         q => q.Range(r => r
                            .Field(f => f.Price)
                            .GreaterThanOrEquals(filterModel.MinPrice)
                            .LessThanOrEquals(filterModel.MaxPrice)
                         ),
                         // Year range query
                         q => q.Range(r => r
                            .Field(f => f.Year)
                            .GreaterThanOrEquals(filterModel.MinYear)
                            .LessThanOrEquals(filterModel.MaxYear)
                         ),
                         // Radiuses query
                         q => filterModel.Radiuses == null
                             ? null
                             : q.Terms(t => t
                                    .Field(f => f.Radius)
                                    .Terms(filterModel.Radiuses)),
                         // Styles query
                         q => filterModel.Styles == null
                             ? null
                             : q.Nested(c => c
                                .Path(e => e.Styles)
                                .Query(q => q
                                    .Terms(terms => terms
                                        .Field(field => field.Styles.First().Slug.Suffix("keyword"))
                                        .Terms(filterModel.Styles)))),
                         // Artist query
                         q => filterModel.Artists == null
                             ? null
                             : q.Nested(c => c
                                .Path(e => e.Artist)
                                .Query(q => q
                                    .Terms(terms => terms
                                        .Field(field => field.Artist.Slug.Suffix("keyword"))
                                        .Terms(filterModel.Artists)))
                               ),
                         // Countries query
                         q => filterModel.Countries == null
                             ? null
                             : q.Nested(c => c
                                .Path(e => e.Country)
                                .Query(q => q
                                    .Terms(terms => terms
                                        .Field(field => field.Country.Slug.Suffix("keyword"))
                                        .Terms(filterModel.Countries)))),
                         // Labels query
                         q => filterModel.Labels == null
                             ? null
                             : q.Nested(c => c
                                .Path(e => e.Label)
                                .Query(q => q
                                    .Terms(terms => terms
                                        .Field(field => field.Label.Slug.Suffix("keyword"))
                                        .Terms(filterModel.Labels))))
                     )
                 )
             )
         );

            return searchResponse.Documents;
        }

        public async Task UpdateAsync(Guid id, RecordModel model, CancellationToken cancellationToken = default)
        {
            var record = await _repository.Records.GetByIdGraphIncludedAsync(id, cancellationToken);

            if (record == null)
            {
                throw new ArgumentException($"Failed to update: no record with id {id}");
            }

            _mapper.Map(model, record);

            await _validator.ValidateAndThrowAsync(record, cancellationToken);

            await _repository.Records.UpdateAsync(record, cancellationToken);
            await _repository.CommitAsync();

            var recordDTO = _mapper.Map<RecordDTO>(record);
            await _elasticClient.UpdateAsync<RecordDTO>(recordDTO.Id, i => i.Doc(recordDTO).Index(_elasticsearchConfig.Index));
        }

        public async Task<IEnumerable<RecordDTO>> SearchAsync(string text, CancellationToken cancellationToken = default)
        {
            var searchResponse = await _elasticClient.SearchAsync<RecordDTO>(s => s
                .Query(q => q
                    .CombinedFields(c => c
                    .Fields(f => f
                        .Field(r => r.Name)
                        .Field(r => r.Description)
                        .Field(r => r.Artist.Name)
                        .Field(r => r.Country.Name)
                        .Field(r => r.Label.Name)
                        .Field(r => r.Styles.First().Name))

                    .Query(text)
                    .Operator(Operator.Or)
                    .MinimumShouldMatch("2")
                    .ZeroTermsQuery(ZeroTermsQuery.All)
                    .AutoGenerateSynonymsPhraseQuery(false)
                   )
                    ));


            if (!searchResponse.IsValid)
            {
                searchResponse.OriginalException.Rethrow();
            }
            return searchResponse.Documents;
        }
    }
}
