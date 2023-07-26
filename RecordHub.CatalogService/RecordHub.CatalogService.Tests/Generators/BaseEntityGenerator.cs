using Bogus;
using RecordHub.CatalogService.Domain.Entities;
using RecordHub.CatalogService.Domain.Models;

namespace RecordHub.CatalogService.Tests.Generators
{
    internal class BaseEntityGenerator
    {

        private Faker<BaseEntity> _faker;

        public BaseEntityGenerator()
        {
            Randomizer.Seed = new Random(123);

            _faker = new Faker<BaseEntity>()
                .RuleFor(a => a.Id, f => f.Random.Uuid())
                .RuleFor(a => a.Name, f => f.Name.FirstName())
                .RuleFor(a => a.Slug, (f, a) => f.Internet.UserName(a.Name));
        }

        public BaseEntity GenerateBaseEntity() => _faker.Generate();

        public IEnumerable<BaseEntity> GenerateBaseEntities() => _faker.GenerateBetween(0, 10);


        public LabelModel GenerateLabelModel()
        {
            var baseEntity = _faker.Generate();

            var model = new LabelModel
            {
                Name = baseEntity.Name,
                Slug = baseEntity.Slug,
            };

            return model;
        }

        public CountryModel GenerateCountryModel()
        {
            var baseEntity = _faker.Generate();

            var model = new CountryModel
            {
                Name = baseEntity.Name,
                Slug = baseEntity.Slug,
            };

            return model;
        }

        public StyleModel GenerateStyleModel()
        {
            var baseEntity = _faker.Generate();

            var model = new StyleModel
            {
                Name = baseEntity.Name,
                Slug = baseEntity.Slug,
            };

            return model;
        }
    }
}
