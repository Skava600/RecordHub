namespace RecordHub.CatalogService.Domain.Models
{
    public class RecordModel
    {
        public string? Name { get; set; }
        public string? Slug { get; set; }
        public short? Radius { get; set; }
        public int? Year { get; set; }
        public string? Description { get; set; }
        public double? Price { get; set; }
        public string? Label { get; set; }
        public IList<string> Styles { get; set; } = new List<string>();
        public string? Country { get; set; }
        public string? Artist { get; set; }
    }
}
