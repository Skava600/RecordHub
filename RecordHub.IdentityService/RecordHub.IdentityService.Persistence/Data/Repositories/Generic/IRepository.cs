using RecordHub.IdentityService.Domain.Data.Entities;

namespace RecordHub.IdentityService.Persistence.Data.Repositories.Generic
{
    public interface IRepository<TEntity> where TEntity : class, IBaseEntity
    {
        Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
        void Update(TEntity entity);
        Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
