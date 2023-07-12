namespace RecordHub.CatalogService.Domain.Models
{
    public class RecordFilterModel
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int? MinPrice { get; set; }
        public int? MaxPrice { get; set; }
        public int? MinYear { get; set; }
        public int? MaxYear { get; set; }
        public IEnumerable<short>? Radiuses { get; set; }
        public IEnumerable<string>? Styles { get; set; }
        public IEnumerable<string>? Artists { get; set; }
        public IEnumerable<string>? Countries { get; set; }
        public IEnumerable<string>? Labels { get; set; }
    }
}
