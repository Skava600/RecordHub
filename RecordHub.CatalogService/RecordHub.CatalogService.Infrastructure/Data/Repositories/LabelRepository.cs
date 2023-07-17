using RecordHub.CatalogService.Application.Data.Repositories;
using RecordHub.CatalogService.Domain.Entities;

namespace RecordHub.CatalogService.Infrastructure.Data.Repositories
{
    public class LabelRepository : BaseRepository<Label>, ILabelRepository
    {
        public LabelRepository(ApplicationDbContext context)
            : base(context)
        {
        }
    }
}
