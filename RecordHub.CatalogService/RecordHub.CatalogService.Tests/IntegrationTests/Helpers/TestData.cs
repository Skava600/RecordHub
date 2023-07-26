using RecordHub.CatalogService.Domain.Entities;
using RecordHub.CatalogService.Domain.Models;
using Record = RecordHub.CatalogService.Domain.Entities.Record;

namespace RecordHub.CatalogService.Tests.IntegrationTests.Helpers
{
    public static class TestData
    {
        public static List<Artist> Artists { get; } = new List<Artist>
        {
            new Artist { Id = Guid.NewGuid(), Name = "Rammstein", Slug = "rammstein" },
            new Artist { Id = Guid.NewGuid(), Name = "AC/DC", Slug = "ac-dc" },
            new Artist { Id = Guid.NewGuid(), Name = "Metallica", Slug = "metallica" },
            new Artist { Id = Guid.NewGuid(), Name = "Pink Floyd", Slug = "pink-floyd" },
            new Artist { Id = Guid.NewGuid(), Name = "Artist to delete", Slug = "delete-artist" },
        };

        public static ArtistModel ArtistModelForCreating { get; } = new ArtistModel
        {
            Slug = "grazhdanskaya-oborona",
            Name = "Grazhdanskaya oborona"
        };

        public static List<Label> Labels { get; } = new List<Label>
        {
            new Label { Id = Guid.NewGuid(), Name = "Music of life", Slug = "music-of-life" },
            new Label { Id = Guid.NewGuid(), Name = "Melodija", Slug = "melodija" },
            new Label { Id = Guid.NewGuid(),Name = "Freestyle", Slug = "freestyle" },
            new Label { Id = Guid.NewGuid(), Name = "Grand Jury", Slug = "grand-jury" },
            new Label { Id = Guid.NewGuid(), Name = "Label to delete", Slug = "delete-label" },
        };

        public static LabelModel LabelModelForCreating { get; } = new LabelModel
        {
            Slug = "ats",
            Name = "Ats",
        };

        public static LabelModel InvalidLabelModel { get; } = new LabelModel
        {
            Slug = "a",
            Name = "A",
        };

        public static List<Country> Countries { get; } = new List<Country>
        {
            new Country { Id = Guid.NewGuid(), Name = "United States", Slug = "united-states" },
            new Country { Id = Guid.NewGuid(), Name = "United Kingdom", Slug = "united-kingdom" },
            new Country { Id = Guid.NewGuid(), Name = "Germany", Slug = "germany" },
            new Country { Id = Guid.NewGuid(), Name = "France", Slug = "france" },
            new Country { Id = Guid.NewGuid(), Name = "Country to delete", Slug = "delete-country" },
            };

        public static CountryModel CountryModelForCreating { get; } = new CountryModel
        {
            Slug = "russia",
            Name = "Russia"
        };

        public static CountryModel InvalidCountryModel { get; } = new CountryModel
        {
            Slug = "i",
            Name = "I"
        };

        public static List<Style> Styles { get; } = new List<Style>
        {
            new Style { Id = Guid.NewGuid(), Name = "Rock", Slug = "rock" },
            new Style { Id = Guid.NewGuid(), Name = "Pop", Slug = "pop" },
            new Style { Id = Guid.NewGuid(), Name = "Hip Hop", Slug = "hip-hop" },
            new Style { Id = Guid.NewGuid(), Name = "Jazz", Slug = "jazz" },
            new Style { Id = Guid.NewGuid(), Name = "Style to delete", Slug = "delete-style" },
        };

        public static StyleModel StyleModelForCreating { get; } = new StyleModel
        {
            Slug = "russian-rock",
            Name = "Russian Rock"
        };

        public static StyleModel InvalidStyleModel { get; } = new StyleModel
        {
            Slug = "i",
            Name = "I"
        };

        public static List<Record> Records { get; } = new List<Record>
        {
            new Record
            {
                Id = Guid.NewGuid(),
                Name = "Record 1",
                Slug = "record-one",
                Radius = 100,
                Year = 2020,
                Description = "Description of Record 1",
                Price = 19.99,
                LabelId = Labels[0].Id,
                CountryId = Countries[0].Id,
                ArtistId = Artists[0].Id,
                Styles = new List<Style>
                {
                    Styles[0],
                    Styles[1]
                }
            },
            new Record
            {
                Id = Guid.NewGuid(),
                Name = "Record 2",
                Slug = "record-two",
                Radius = 150,
                Year = 2019,
                Description = "Description of Record 2",
                Price = 24.99,
                LabelId = Labels[1].Id,
                CountryId = Countries[1].Id,
                ArtistId = Artists[1].Id,
                Styles = new List<Style>
                {
                    Styles[2],
                    Styles[3]
                }
            },
             new Record
            {
                Id = Guid.NewGuid(),
                Name = "Record for deleting",
                Slug = "delete-record",
                Radius = 150,
                Year = 2019,
                Description = "Description of Record for delete",
                Price = 24.99,
                LabelId = Labels[1].Id,
                CountryId = Countries[1].Id,
                ArtistId = Artists[1].Id,
                Styles = new List<Style>
                {
                    Styles[2],
                    Styles[3]
                }
            },
        };

        public static RecordModel RecordModelForCreating { get; } = new RecordModel
        {
            Name = "Record 3",
            Slug = "grazhdanskaya-oborona",
            Radius = 12,
            Year = 2020,
            Description = "Description of Record 3",
            Price = 15.99,
            Label = Labels[2].Slug,
            Country = Countries[2].Slug,
            Artist = Artists[2].Slug,
            Styles = new List<string>
                {
                    Styles[1].Slug,
                    Styles[3].Slug
                }
        };

        public static RecordModel InvalidRecordModel { get; } = new RecordModel
        {
            Name = "",
            Slug = "r",
            Radius = -2,
            Year = 2030,
            Description = "Description of Record 3",
            Price = -15.99,
            Label = Labels[2].Slug,
            Country = Countries[2].Slug,
            Artist = Artists[2].Slug,
            Styles = new List<string>
                {
                    Styles[1].Slug,
                    Styles[3].Slug
                }
        };
    }
}
