using AutoMapper;
using FluentValidation;
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
        public StyleCatalogService(IMapper mapper, IUnitOfWork repository, IValidator<BaseEntity> validator)
        {
            _mapper = mapper;
            _repository = repository;
            _validator = validator;
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
            await _repository.Styles.DeleteAsync(id, cancellationToken);
            await _repository.CommitAsync();
        }

        public async Task<IEnumerable<StyleDTO>> GetAllAsync(CancellationToken cancellationToken)
        {
            var styles = await _repository.Styles.GetAllAsync(cancellationToken);

            return _mapper.Map<IEnumerable<StyleDTO>>(styles);
        }

        public async Task UpdateAsync(Guid id, StyleModel model, CancellationToken cancellationToken)
        {
            var style = await _repository.Styles.GetByIdAsync(id, cancellationToken);

            if (style == null)
            {
                throw new ArgumentException($"Failed to update: no artist with id {id}");
            }

            _mapper.Map(model, style);

            await _validator.ValidateAndThrowAsync(style, cancellationToken);

            await _repository.Styles.UpdateAsync(style, cancellationToken);
            await _repository.CommitAsync();
        }
    }
}
