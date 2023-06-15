namespace RecordHub.CatalogService.Domain.Entities
{
    public class Style : BaseEntity
    {
        public ICollection<RecordStyle> RecordStyles { get; set; }
    }
}
