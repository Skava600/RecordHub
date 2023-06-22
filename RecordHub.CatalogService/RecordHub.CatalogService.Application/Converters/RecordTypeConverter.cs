using AutoMapper;
using RecordHub.CatalogService.Application.Data;
using RecordHub.CatalogService.Application.Exceptions;
using RecordHub.CatalogService.Domain.Entities;
using RecordHub.CatalogService.Domain.Models;

namespace RecordHub.CatalogService.Application.Converters
{
    public class RecordTypeConverter : ITypeConverter<RecordModel, Record>
    {
        private readonly IUnitOfWork _repository;

        public RecordTypeConverter(IUnitOfWork repository)
        {
            _repository = repository;
        }
        public Record Convert(RecordModel source, Record destination, ResolutionContext context)
        {
            destination ??= new Record();
            destination.Radius = source.Radius;
            destination.Description = source.Description;
            destination.Name = source.Name;
            destination.Slug = source.Slug;
            destination.Price = source.Price;
            destination.Year = source.Year;

            var artist = _repository.Artists.GetBySlugAsync(source.Artist).Result;
            if (artist == null)
            {
                throw new EntityNotFoundException(nameof(source.Artist));
            }

            var country = _repository.Countries.GetBySlugAsync(source.Country).Result;
            if (country == null)
            {
                throw new EntityNotFoundException(nameof(source.Country));
            }

            var label = _repository.Labels.GetBySlugAsync(source.Label).Result;
            if (label == null)
            {
                throw new EntityNotFoundException(nameof(source.Label));
            }

            destination.Styles = new List<Style>();
            foreach (var styleSlug in source.Styles)
            {
                var style = _repository.Styles.GetBySlugAsync(styleSlug).Result;
                if (style == null)
                    throw new EntityNotFoundException(nameof(source.Styles));

                destination.Styles.Add(style);
            }

            destination.Label = label;
            destination.Country = country;
            destination.ArtistId = artist.Id;

            return destination;
        }
    }
}
