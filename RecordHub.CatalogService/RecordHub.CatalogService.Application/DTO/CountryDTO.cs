using System.Text.Json.Serialization;

namespace RecordHub.CatalogService.Application.DTO
{
    public class CountryDTO
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("slug")]
        public string Slug { get; set; }
    }
}
