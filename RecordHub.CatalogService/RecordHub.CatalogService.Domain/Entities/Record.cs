namespace RecordHub.CatalogService.Domain.Entities
{
    public class Record : BaseEntity
    {
        public short Radius { get; set; }
        public int Year { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public Label Label { get; set; }
        public ICollection<RecordStyle> RecordStyles { get; set; }
        public Language Language { get; set; }
        public Artist Artist { get; set; }

    }
}
