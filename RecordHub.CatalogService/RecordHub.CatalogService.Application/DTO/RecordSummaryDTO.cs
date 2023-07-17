namespace RecordHub.CatalogService.Application.DTO
{
    public class RecordSummaryDTO
    {
        public string Name { get; set; }
        public string Slug { get; set; }
        public double Price { get; set; }
        public IList<StyleDTO> Styles { get; set; }
    }
}
