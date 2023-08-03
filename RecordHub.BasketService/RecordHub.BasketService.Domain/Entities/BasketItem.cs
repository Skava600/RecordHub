using System.Text.Json.Serialization;

namespace RecordHub.BasketService.Domain.Entities
{
    public class BasketItem
    {
        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }
        [JsonPropertyName("price")]
        public double Price { get; set; }
        [JsonPropertyName("productid")]
        public string ProductId { get; set; }
        [JsonPropertyName("productname")]
        public string ProductName { get; set; }
    }
}
