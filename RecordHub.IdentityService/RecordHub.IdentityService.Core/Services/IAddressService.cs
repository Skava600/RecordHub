using RecordHub.IdentityService.Domain.Data.Entities;
using RecordHub.IdentityService.Domain.Models;

namespace RecordHub.IdentityService.Core.Services
{
    public interface IAddressService
    {
        Task<Address> AddAsync(
            string? userId,
            AddressModel model,
            CancellationToken cancellationToken = default);

        Task DeleteAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<Address> UpdateAsync(
            Guid id,
            AddressModel model,
            CancellationToken cancellationToken = default);
    }
}
