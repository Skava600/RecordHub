using AutoMapper;
using FluentValidation;
using Nest;
using RecordHub.CatalogService.Application.Data;
using RecordHub.CatalogService.Application.DTO;
using RecordHub.CatalogService.Application.Services;
using RecordHub.CatalogService.Domain.Entities;
using RecordHub.CatalogService.Domain.Models;
using RecordHub.Shared.Exceptions;

namespace RecordHub.CatalogService.Infrastructure.Services
{
    public class ArtistCatalogService : IArtistCatalogService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _repository;
        private readonly IValidator<BaseEntity> _validator;
        private readonly IElasticClient _elasticClient;

        public ArtistCatalogService(
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

        public async Task AddAsync(ArtistModel model, CancellationToken cancellationToken)
        {
            var artist = _mapper.Map<Artist>(model);

            await _validator.ValidateAndThrowAsync(artist, cancellationToken);

            await _repository.Artists.AddAsync(artist);
            await _repository.CommitAsync();
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            var artist = await _repository.Artists.DeleteAsync(id, cancellationToken);
            if (artist != null)
            {
                await _repository.CommitAsync();

                await _elasticClient.DeleteByQueryAsync<RecordDTO>(q => q.Query(rq => rq
                        .Wildcard(t => t
                        .Field(r => r.Artist.Slug.Suffix("keyword"))
                        .Wildcard(artist.Slug)
                        .Rewrite(MultiTermQueryRewrite.TopTermsBoost(10)))));
            }

        }

        public async Task<ArtistDTO> GetBySlug(string slug, CancellationToken cancellationToken)
        {
            var artist = await _repository.Artists.GetBySlugAsync(slug, cancellationToken);
            if (artist == null)
            {
                throw new EntityNotFoundException(nameof(slug));
            }

            return _mapper.Map<ArtistDTO>(artist);
        }

        public async Task UpdateAsync(
            Guid id,
            ArtistModel model,
            CancellationToken cancellationToken)
        {
            var artist = await _repository.Artists.GetByIdAsync(id, cancellationToken);

            if (artist == null)
            {
                throw new ArgumentException($"Failed to update: no artist with id {id}");
            }

            _mapper.Map(model, artist);

            await _validator.ValidateAndThrowAsync(artist, cancellationToken);

            await _repository.Artists.UpdateAsync(artist, cancellationToken);


            var records = await _repository.Records.GetArtistsRecordsAsync(artist.Id);
            var recordsDTO = _mapper.Map<IEnumerable<RecordDTO>>(records);

            await _repository.CommitAsync();
            if (recordsDTO.Any())
            {
                await _elasticClient.IndexManyAsync(recordsDTO);
            }
        }
    }
}
