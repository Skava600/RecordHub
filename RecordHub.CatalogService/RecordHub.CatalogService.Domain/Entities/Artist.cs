namespace RecordHub.CatalogService.Domain.Entities
{
    public class Artist : BaseEntity
    {
        public ICollection<Record> Records { get; set; }
    }
}
