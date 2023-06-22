using AutoMapper;
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
        public CountryCatalogService(IMapper mapper, IUnitOfWork repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public async Task AddAsync(CountryModel model, CancellationToken cancellationToken)
        {
            var country = _mapper.Map<Country>(model);

            await _repository.Countries.AddAsync(country);
            await _repository.CommitAsync();
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            await _repository.Countries.DeleteAsync(id, cancellationToken);
            await _repository.CommitAsync();
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
            await _repository.Countries.UpdateAsync(country, cancellationToken);
            await _repository.CommitAsync();
        }
    }
}
