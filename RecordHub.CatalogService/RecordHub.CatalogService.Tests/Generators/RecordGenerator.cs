using Bogus;
using RecordHub.CatalogService.Application.DTO;
using RecordHub.CatalogService.Domain.Entities;
using RecordHub.CatalogService.Domain.Models;
using Record = RecordHub.CatalogService.Domain.Entities.Record;
namespace RecordHub.CatalogService.Tests.Generators
{
    public class RecordGenerator
    {
        private Faker<RecordDTO> _fakerDTO;
        private Faker<RecordModel> _fakerModel;
        private Faker<Record> _faker;

        public RecordGenerator()
        {
            Randomizer.Seed = new Random(123);
            _fakerDTO = new Faker<RecordDTO>()
               .StrictMode(true)
               .RuleFor(r => r.Id, f => Guid.NewGuid())
               .RuleFor(r => r.Radius, f => f.Random.Short())
               .RuleFor(r => r.Name, f => f.Music.Random.Word())
               .RuleFor(r => r.Description, f => f.Lorem.Random.Words(10))
               .RuleFor(r => r.Price, f => f.Random.Double())
               .RuleFor(r => r.Year, f => f.Random.Int(1900, 2023))
               .RuleFor(r => r.Slug, (f, r) => f.Internet.UserName(r.Name))
               .RuleFor(r => r.Label, (f, r) => new LabelDTO { Name = f.Random.Word(), Slug = f.Internet.DomainName() })
               .RuleFor(r => r.Country, (f, r) => new CountryDTO { Name = f.Random.Word(), Slug = f.Internet.DomainName() })
               .RuleFor(r => r.Artist, (f, r) => new ArtistDTO { Name = f.Name.FullName(), Slug = f.Name.FirstName() })
               .RuleFor(r => r.Styles, (f, r) => new List<StyleDTO> { new StyleDTO { Name = f.Music.Genre(), Slug = f.Music.Genre() } });

            _fakerModel = new Faker<RecordModel>()
               .StrictMode(true)
               .RuleFor(r => r.Radius, f => f.Random.Short())
               .RuleFor(r => r.Name, f => f.Music.Random.Word())
               .RuleFor(r => r.Description, f => f.Lorem.Random.Words(10))
               .RuleFor(r => r.Price, f => f.Random.Double())
               .RuleFor(r => r.Year, f => f.Random.Int(1900, 2023))
               .RuleFor(r => r.Slug, (f, r) => f.Internet.UserName(r.Name))
               .RuleFor(r => r.Label, (f, r) => f.Random.Word())
               .RuleFor(r => r.Country, (f, r) => f.Random.Word())
               .RuleFor(r => r.Artist, (f, r) => f.Random.Word())
               .RuleFor(r => r.Styles, (f, r) => new List<string> { f.Music.Random.Word(), f.Music.Random.Word() });

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
            return _fakerDTO.Generate();
        }

        public RecordModel GenerateRecordModel()
        {
            return _fakerModel.Generate();
        }
        public Record GenerateRecord()
        {
            return _faker.Generate();
        }
    }
}
