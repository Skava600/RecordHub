using System.Text.Json.Serialization;

namespace RecordHub.CatalogService.Application.DTO
{
    public class ArtistDTO
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("slug")]
        public string Slug { get; set; }
        [JsonPropertyName("records")]
        public IList<RecordSummaryDTO> Records { get; set; }
    }
}
