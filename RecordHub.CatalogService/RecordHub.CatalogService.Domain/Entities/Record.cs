namespace RecordHub.CatalogService.Domain.Entities
{
    public class Record : BaseEntity
    {
        public short Radius { get; set; }
        public int Year { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public Label Label { get; set; }
        public Guid LabelId { get; set; }
        public IList<Style> Styles { get; set; } = new List<Style>();
        public Country Country { get; set; }
        public Guid CountryId { get; set; }
        public Artist Artist { get; set; }
        public Guid ArtistId { get; set; }
    }
}
