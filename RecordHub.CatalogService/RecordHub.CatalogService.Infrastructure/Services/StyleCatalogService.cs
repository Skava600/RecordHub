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
    public class StyleCatalogService : IStyleCatalogService
    {
        private readonly IMapper _mapper;

        private readonly IUnitOfWork _repository;
        private readonly IValidator<BaseEntity> _validator;
        private readonly IElasticClient _elasticClient;
        public StyleCatalogService(
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

        public async Task AddAsync(StyleModel model, CancellationToken cancellationToken)
        {
            var style = _mapper.Map<Style>(model);

            await _validator.ValidateAndThrowAsync(style, cancellationToken);

            await _repository.Styles.AddAsync(style, cancellationToken);
            await _repository.CommitAsync();
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            var style = await _repository.Styles.DeleteAsync(id, cancellationToken);
            if (style != null)
            {
                //await _repository.CommitAsync();

                await _elasticClient.DeleteByQueryAsync<RecordDTO>(q => q.Query(rq => rq
                        .Wildcard(t => t
                        .Field(r => r.Styles.First().Slug.Suffix("keyword"))
                        .Wildcard(style.Slug)
                        .Rewrite(MultiTermQueryRewrite.TopTermsBoost(10)))));
            }
        }

        public async Task<IEnumerable<StyleDTO>> GetAllAsync(CancellationToken cancellationToken)
        {
            var styles = await _repository.Styles.GetAllAsync(cancellationToken);

            return _mapper.Map<IEnumerable<StyleDTO>>(styles);
        }

        public async Task UpdateAsync(Guid id, StyleModel model, CancellationToken cancellationToken)
        {
            var style = await _repository.Styles.GetByIdIncludedGraph(id, cancellationToken);

            if (style == null)
            {
                throw new ArgumentException($"Failed to update: no artist with id {id}");
            }

            _mapper.Map(model, style);

            await _validator.ValidateAndThrowAsync(style, cancellationToken);

            await _repository.Styles.UpdateAsync(style, cancellationToken);

            if (style.Records.Any())
            {
                var recordsDto = _mapper.Map<IEnumerable<RecordDTO>>(style.Records);

                await _elasticClient.IndexManyAsync(recordsDto);
            }
            await _repository.CommitAsync();
        }
    }
}
