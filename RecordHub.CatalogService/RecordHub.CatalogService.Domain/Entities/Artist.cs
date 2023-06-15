namespace RecordHub.CatalogService.Domain.Entities
{
    public class Artist : BaseEntity
    {
        public ICollection<Record> Artists { get; set; }
    }
}
