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
    internal class LabelCatalogService : ILabelCatalogService
    {
        private readonly IMapper _mapper;

        private readonly IUnitOfWork _repository;
        private readonly IElasticClient _elasticClient;
        private readonly IValidator<BaseEntity> _validator;
        public LabelCatalogService(
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

        public async Task AddAsync(LabelModel model, CancellationToken cancellationToken)
        {
            var label = _mapper.Map<Label>(model);

            await _validator.ValidateAndThrowAsync(label, cancellationToken);

            await _repository.Labels.AddAsync(label);
            await _repository.CommitAsync();
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            var label = await _repository.Labels.DeleteAsync(id, cancellationToken);
            if (label != null)
            {
                await _repository.CommitAsync();

                await _elasticClient.DeleteByQueryAsync<RecordDTO>(q => q.Query(rq => rq
                        .Wildcard(t => t
                        .Field(r => r.Label.Slug.Suffix("keyword"))
                        .Wildcard(label.Slug)
                        .Rewrite(MultiTermQueryRewrite.TopTermsBoost(10)))));
            }
        }

        public async Task<IEnumerable<LabelDTO>> GetAllAsync(CancellationToken cancellationToken)
        {
            var labels = await _repository.Labels.GetAllAsync(cancellationToken);

            return _mapper.Map<IEnumerable<LabelDTO>>(labels);
        }

        public async Task UpdateAsync(Guid id, LabelModel model, CancellationToken cancellationToken)
        {
            var label = await _repository.Labels.GetByIdAsync(id, cancellationToken);

            if (label == null)
            {
                throw new ArgumentException($"Failed to update: no artist with id {id}");
            }

            _mapper.Map(model, label);
            await _repository.Labels.UpdateAsync(label, cancellationToken);
            await _repository.CommitAsync();

            var records = await _repository.Records.GetLabelsRecordsAsync(label.Id);
            var recordsDTO = _mapper.Map<IEnumerable<RecordDTO>>(records);
            if (recordsDTO.Any())
                await _elasticClient.IndexManyAsync(recordsDTO);
        }
    }
}
