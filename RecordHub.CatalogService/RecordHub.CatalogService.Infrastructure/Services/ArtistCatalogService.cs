﻿using AutoMapper;
using FluentValidation;
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
        public ArtistCatalogService(IMapper mapper, IUnitOfWork repository, IValidator<BaseEntity> validator)
        {
            _mapper = mapper;
            _repository = repository;
            _validator = validator;
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
            await _repository.Artists.DeleteAsync(id, cancellationToken);
            await _repository.CommitAsync();
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

        public async Task UpdateAsync(Guid id, ArtistModel model, CancellationToken cancellationToken)
        {
            var artist = await _repository.Artists.GetByIdAsync(id, cancellationToken);

            if (artist == null)
            {
                throw new ArgumentException($"Failed to update: no artist with id {id}");
            }

            _mapper.Map(model, artist);

            await _validator.ValidateAndThrowAsync(artist, cancellationToken);

            await _repository.Artists.UpdateAsync(artist, cancellationToken);
            await _repository.CommitAsync();
        }
    }
}
