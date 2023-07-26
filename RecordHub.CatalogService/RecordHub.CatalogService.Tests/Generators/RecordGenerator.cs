using Bogus;
using RecordHub.CatalogService.Application.DTO;
using RecordHub.CatalogService.Domain.Entities;
using RecordHub.CatalogService.Domain.Models;
using Record = RecordHub.CatalogService.Domain.Entities.Record;
namespace RecordHub.CatalogService.Tests.Generators
{
    public class RecordGenerator
    {
        private Faker<Record> _faker;

        public RecordGenerator()
        {
            Randomizer.Seed = new Random(123);

            _faker = new Faker<Record>()
               .StrictMode(true)
               .RuleFor(r => r.Id, f => Guid.NewGuid())
               .RuleFor(r => r.LabelId, f => f.Random.Uuid())
               .RuleFor(r => r.CountryId, f => f.Random.Uuid())
               .RuleFor(r => r.ArtistId, f => f.Random.Uuid())
               .RuleFor(r => r.Radius, f => f.Random.Short())
               .RuleFor(r => r.Name, f => f.Music.Random.Word())
               .RuleFor(r => r.Description, f => f.Lorem.Random.Words(10))
               .RuleFor(r => r.Price, f => f.Random.Double())
               .RuleFor(r => r.Year, f => f.Random.Int(1900, 2023))
               .RuleFor(r => r.Slug, (f, r) => f.Internet.UserName(r.Name))
               .RuleFor(r => r.Label, (f, r) => new Label { Id = r.LabelId, Name = f.Random.Word(), Slug = f.Internet.DomainName() })
               .RuleFor(r => r.Country, (f, r) => new Country { Id = r.CountryId, Name = f.Random.Word(), Slug = f.Internet.DomainName() })
               .RuleFor(r => r.Artist, (f, r) => new Artist { Id = r.ArtistId, Name = f.Name.FullName(), Slug = f.Name.FirstName() })
               .RuleFor(r => r.Styles, (f, r) => new List<Style> { new Style { Id = f.Random.Uuid(), Name = f.Music.Genre(), Slug = f.Music.Genre() } });
        }

        public RecordDTO GenerateRecordDTO()
        {
            var record = _faker.Generate();
            var recordDto = new RecordDTO
            {
                Id = record.Id,
                Description = record.Description,
                Artist = new ArtistDTO { Name = record.Artist.Name, Slug = record.Artist.Slug },
                Price = record.Price,
                Name = record.Name,
                Slug = record.Slug,
                Radius = record.Radius,
                Year = record.Year,
                Country = new CountryDTO { Name = record.Country.Name, Slug = record.Country.Slug },
                Label = new LabelDTO { Name = record.Label.Name, Slug = record.Label.Slug },
                Styles = record.Styles.Select(s => new StyleDTO { Name = s.Name, Slug = s.Slug }).ToArray()
            };

            return recordDto;
        }

        public RecordModel GenerateRecordModel()
        {
            var record = _faker.Generate();
            var model = new RecordModel
            {
                Name = record.Name,
                Slug = record.Slug,
                Year = record.Year,
                Description = record.Description,
                Price = record.Price,
                Radius = record.Radius,
                Label = record.Label.Slug,
                Artist = record.Artist.Slug,
                Country = record.Country.Slug,
                Styles = record.Styles.Select(a => a.Slug).ToArray()
            };

            return model;
        }

        public Record GenerateRecord()
        {
            return _faker.Generate();
        }
    }
}
