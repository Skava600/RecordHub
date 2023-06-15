namespace RecordHub.CatalogService.Domain.Entities
{
    public class BaseEntity
    {
        public Guid Id { get; set; }
        public string Slug { get; set; }
        public string Name { get; set; }
    }
}
