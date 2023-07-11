using AutoMapper;
using RecordHub.CatalogService.Application.Data;
using RecordHub.CatalogService.Domain.Entities;
using RecordHub.CatalogService.Domain.Models;
using RecordHub.Shared.Exceptions;

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
            destination.Radius = source.Radius ?? destination.Radius;
            destination.Description = source.Description ?? destination.Description;
            destination.Name = source.Name ?? destination.Name;
            destination.Slug = source.Slug ?? destination.Slug;
            destination.Price = source.Price ?? destination.Price;
            destination.Year = source.Year ?? destination.Year;
            var artist =
                    source.Artist != null ?
                    _repository.Artists.GetBySlugAsync(source.Artist).Result : null;
            if (artist == null && source.Artist != null)
            {
                throw new EntityNotFoundException(nameof(source.Artist));
            }

            var country =
                        source.Country != null ?
                        _repository.Countries.GetBySlugAsync(source.Country).Result : null;
            if (country == null && source.Country != null)
            {
                throw new EntityNotFoundException(nameof(source.Country));
            }

            var label =
                    source.Label != null ?
                    _repository.Labels.GetBySlugAsync(source.Label).Result : null;
            if (label == null && source.Label != null)
            {
                throw new EntityNotFoundException(nameof(source.Label));
            }

            if (source.Styles.Count() > 0)
            {
                destination.Styles = new List<Style>();
            }
            foreach (var styleSlug in source.Styles)
            {
                var style = _repository.Styles.GetBySlugAsync(styleSlug).Result;
                if (style == null)
                    throw new EntityNotFoundException(nameof(source.Styles));

                destination.Styles.Add(style);
            }

            destination.Label = label ?? destination.Label;
            destination.Country = country ?? destination.Country;
            destination.Artist = artist ?? destination.Artist;
            destination.ArtistId = artist != null ? artist.Id : destination.ArtistId;
            return destination;
        }
    }
}
