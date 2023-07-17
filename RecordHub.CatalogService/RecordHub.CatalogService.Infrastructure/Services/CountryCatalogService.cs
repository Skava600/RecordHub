using AutoMapper;
using FluentValidation;
using Nest;
using RecordHub.CatalogService.Application.Data;
using RecordHub.CatalogService.Application.DTO;
using RecordHub.CatalogService.Application.Services;
using RecordHub.CatalogService.Domain.Entities;
using RecordHub.CatalogService.Domain.Models;

namespace RecordHub.CatalogService.Infrastructure.Services
{
    public class CountryCatalogService : ICountryCatalogService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _repository;
        private readonly IValidator<BaseEntity> _validator;
        private readonly IElasticClient _elasticClient;

        public CountryCatalogService(
            IMapper mapper,
            IUnitOfWork repository,
            IValidator<BaseEntity> validator,
            IElasticClient elasticClient)
        {
            _mapper = mapper;
            _repository = repository;
            _validator = validator;
            _elasticClient = elasticClient;
        }

        public async Task AddAsync(CountryModel model, CancellationToken cancellationToken)
        {
            var country = _mapper.Map<Country>(model);

            await _validator.ValidateAndThrowAsync(country, cancellationToken);

            await _repository.Countries.AddAsync(country, cancellationToken);
            await _repository.CommitAsync();
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            var country = await _repository.Countries.DeleteAsync(id, cancellationToken);
            if (country != null)
            {
                await _repository.CommitAsync();

                await _elasticClient.DeleteByQueryAsync<RecordDTO>(q => q.Query(rq => rq
                        .Wildcard(t => t
                        .Field(r => r.Country.Slug.Suffix("keyword"))
                        .Wildcard(country.Slug)
                        .Rewrite(MultiTermQueryRewrite.TopTermsBoost(10)))));
            }
        }

        public async Task<IEnumerable<CountryDTO>> GetAllAsync(CancellationToken cancellationToken)
        {
            var countries = await _repository.Countries.GetAllAsync(cancellationToken);

            return _mapper.Map<IEnumerable<CountryDTO>>(countries);
        }

        public async Task UpdateAsync(Guid id, CountryModel model, CancellationToken cancellationToken)
        {
            var country = await _repository.Countries.GetByIdAsync(id, cancellationToken);

            if (country == null)
            {
                throw new ArgumentException($"Failed to update: no artist with id {id}");
            }

            _mapper.Map(model, country);

            await _validator.ValidateAndThrowAsync(country, cancellationToken);

            await _repository.Countries.UpdateAsync(country, cancellationToken);

            var records = await _repository.Records.GetCountrysRecordsAsync(country.Id);
            var recordsDTO = _mapper.Map<IEnumerable<RecordDTO>>(records);

            await _repository.CommitAsync();

            if (recordsDTO.Any())
            {
                await _elasticClient.IndexManyAsync(recordsDTO);
            }
        }
    }
}
