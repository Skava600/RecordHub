namespace RecordHub.CatalogService.Domain.Entities
{
    public class Language : BaseEntity
    {
        public ICollection<Record> Records { get; set; }
    }
}
