using AutoMapper;
using Microsoft.AspNetCore.Identity;
using RecordHub.IdentityService.Core.Services;
using RecordHub.IdentityService.Domain.Data.Entities;
using RecordHub.IdentityService.Domain.Enum;
using RecordHub.IdentityService.Domain.Models;
using RecordHub.IdentityService.Persistence.Data.Repositories.Generic;
using RecordHub.Shared.Exceptions;
using System.Security.Claims;

namespace RecordHub.IdentityService.Infrastructure.Services
{
    public class AddressService : IAddressService
    {
        private readonly IAddressRepository _repo;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;

        public AddressService(
            IAddressRepository repo,
            UserManager<User> userManager,
            IMapper mapper)
        {
            _repo = repo;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<Address> AddAsync(
            string? userId,
            AddressModel model,
            CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new UserNotFoundException("Error during adding address: user not found");
            }

            var address = _mapper.Map<Address>(model);
            address.UserId = user.Id;

            await _repo.AddAsync(address, cancellationToken);

            return address;
        }

        public async Task DeleteAsync(
            Guid id,
            ClaimsPrincipal user,
            CancellationToken cancellationToken = default)
        {
            var address = await _repo.GetByIdAsync(id);
            if (address != null && (
                address.UserId.ToString().Equals(user.FindFirst(ClaimTypes.NameIdentifier)?.Value) ||
                user.IsInRole(Roles.Admin.ToString())))
            {
                await _repo.DeleteAsync(address, cancellationToken);
            }
        }

        public async Task<Address> UpdateAsync(
            Guid id,
            AddressModel model,
            ClaimsPrincipal user,
            CancellationToken cancellationToken = default)
        {
            var address = await _repo.GetByIdAsync(id, cancellationToken);
            if (address == null)
            {
                throw new EntityNotFoundException(nameof(id));
            }

            if (!address.UserId.ToString().Equals(user.FindFirst(ClaimTypes.NameIdentifier)?.Value) &&
                !user.IsInRole(Roles.Admin.ToString()))
            {
                throw new UnauthorizedAccessException();
            }

            _mapper.Map(model, address);

            await _repo.Update(address, cancellationToken);

            return address;
        }
    }
}
