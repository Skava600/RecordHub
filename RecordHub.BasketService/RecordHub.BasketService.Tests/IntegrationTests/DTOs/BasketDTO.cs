using RecordHub.BasketService.Domain.Entities;
using System.Text.Json.Serialization;

namespace RecordHub.BasketService.Tests.IntegrationTests.DTOs
{
    public class BasketDTO
    {
        [JsonPropertyName("userName")]
        public string UserName { get; set; }
        [JsonPropertyName("items")]
        public List<BasketItem> Items { get; set; }
        [JsonPropertyName("totalPrice")]
        public double TotalPrice { get; set; }
    }
}
