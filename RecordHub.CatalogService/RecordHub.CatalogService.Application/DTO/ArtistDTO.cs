namespace RecordHub.CatalogService.Application.DTO
{
    public class ArtistDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public IList<RecordSummaryDTO> Records { get; set; }
    }
}
