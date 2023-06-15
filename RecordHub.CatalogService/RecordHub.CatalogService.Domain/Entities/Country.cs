namespace RecordHub.CatalogService.Domain.Entities
{
    public class Country : BaseEntity
    {
        public ICollection<Record> Records { get; set; }
    }
}
