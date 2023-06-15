namespace RecordHub.CatalogService.Domain.Entities
{
    public class Label : BaseEntity
    {
        public ICollection<Record> Records { get; set; }
    }
}
