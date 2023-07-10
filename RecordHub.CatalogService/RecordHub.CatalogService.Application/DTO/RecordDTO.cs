namespace RecordHub.CatalogService.Application.DTO
{
    public class RecordDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public short Radius { get; set; }
        public int Year { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public LabelDTO Label { get; set; }
        public IList<StyleDTO> Styles { get; set; }
        public CountryDTO Country { get; set; }
        public ArtistDTO Artist { get; set; }
    }
}
