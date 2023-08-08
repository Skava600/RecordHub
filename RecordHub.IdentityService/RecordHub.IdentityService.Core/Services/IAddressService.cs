using RecordHub.IdentityService.Domain.Data.Entities;
using RecordHub.IdentityService.Domain.Models;
using System.Security.Claims;

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
            ClaimsPrincipal user,
            CancellationToken cancellationToken = default);

        Task<Address> UpdateAsync(
            Guid id,
            AddressModel model,
            ClaimsPrincipal user,
            CancellationToken cancellationToken = default);
    }
}
