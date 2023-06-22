using AutoMapper;
using RecordHub.CatalogService.Application.Data;
using RecordHub.CatalogService.Application.DTO;
using RecordHub.CatalogService.Application.Exceptions;
using RecordHub.CatalogService.Application.Services;
using RecordHub.CatalogService.Domain.Entities;
using RecordHub.CatalogService.Domain.Models;

namespace RecordHub.CatalogService.Infrastructure.Services
{
    public class RecordCatalogService : IRecordCatalogService
    {
        private readonly IMapper _mapper;

        private readonly IUnitOfWork _repository;

        public RecordCatalogService(IMapper mapper, IUnitOfWork repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        public async Task AddAsync(RecordModel model, CancellationToken cancellationToken = default)
        {
            var record = _mapper.Map<Record>(model);


            await _repository.Records.AddAsync(record);

            await _repository.CommitAsync();
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            await _repository.Records.DeleteAsync(id, cancellationToken);
            await _repository.CommitAsync();
        }

        public async Task<RecordDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var record = await _repository.Records.GetByIdAsync(id, cancellationToken);
            return _mapper.Map<RecordDTO>(record);
        }

        public async Task<IEnumerable<RecordDTO>> GetByPageAsync(int page, int pageSize, CancellationToken cancellationToken = default)
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

        public async Task UpdateAsync(Guid id, RecordModel model, CancellationToken cancellationToken = default)
        {
            var record = await _repository.Records.GetByIdAsync(id, cancellationToken);

            if (record == null)
            {
                throw new ArgumentException($"Failed to update: no record with id {id}");
            }

            _mapper.Map(model, record);
            await _repository.Records.UpdateAsync(record, cancellationToken);
            await _repository.CommitAsync();
        }
    }
}
