using RecordHub.CatalogService.Application.Data.Repositories;
using RecordHub.CatalogService.Domain.Entities;

namespace RecordHub.CatalogService.Infrastructure.Data.Repositories
{
    public class CountryRepository : BaseRepository<Country>, ICountryRepository
    {
        public CountryRepository(ApplicationDbContext context)
            : base(context)
        {
        }
    }
}
