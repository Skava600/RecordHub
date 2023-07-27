using Bogus;
using RecordHub.CatalogService.Application.DTO;
using RecordHub.CatalogService.Domain.Entities;
using RecordHub.CatalogService.Domain.Models;

namespace RecordHub.CatalogService.Tests.Generators
{
    public class ArtistGenerator
    {
        private Faker<ArtistModel> _faker;

        public ArtistGenerator()
        {
            Randomizer.Seed = new Random(123);

            _faker = new Faker<ArtistModel>()
                .RuleFor(a => a.Name, f => f.Name.FirstName())
                .RuleFor(a => a.Slug, (f, a) => f.Internet.UserName(a.Name));
        }

        public ArtistModel GenerateModel() => _faker.Generate();

        public Artist GenerateEntity()
        {
            var model = _faker.Generate();

            var entity = new Artist
            {
                Name = model.Name,
                Slug = model.Slug,
                Id = Guid.Empty
            };

            return entity;
        }

        public ArtistDTO GenerateDTO()
        {
            return new ArtistDTO();
        }
    }
}
