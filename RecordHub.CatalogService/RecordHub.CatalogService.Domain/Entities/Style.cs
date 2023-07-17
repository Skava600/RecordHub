namespace RecordHub.CatalogService.Domain.Entities
{
    public class Style : BaseEntity
    {
        public ICollection<Record> Records { get; set; }
    }
}
