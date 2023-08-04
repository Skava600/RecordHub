using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using RecordHub.IdentityService.Domain.Data.Entities;
using RecordHub.IdentityService.Domain.Models;
using RecordHub.IdentityService.Infrastructure.Services;
using RecordHub.IdentityService.Persistence.Data.Repositories.Generic;
using RecordHub.IdentityService.Tests.Setups;
using RecordHub.Shared.Exceptions;

namespace RecordHub.IdentityService.Tests.UnitTests.Services
{
    public class AddressServiceTests
    {
        private readonly Mock<IAddressRepository> _addressRepositoryMock;
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly AddressService _addressService;

        public AddressServiceTests()
        {
            _addressRepositoryMock = new Mock<IAddressRepository>();
            _userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(),
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null);
            _mapperMock = new Mock<IMapper>();
            _addressService = new AddressService(
                _addressRepositoryMock.Object,
                _userManagerMock.Object,
                _mapperMock.Object
            );
        }

        [Fact]
        public async Task AddAsync_ThrowsUserNotFoundException_WhenUserNotFound()
        {
            // Arrange
            string userId = "nonexistentUserId";
            var model = new AddressModel();
            var cancellationToken = CancellationToken.None;

            _userManagerMock.SetupFindByIdAsync(null);

            // Act
            Func<Task> act = async () => await _addressService.AddAsync(userId, model, cancellationToken);

            // Assert
            await act.Should().ThrowAsync<UserNotFoundException>();
        }

        [Fact]
        public async Task AddAsync_CallsAddAsyncOnRepository_WithMappedAddress()
        {
            // Arrange
            string userId = "existingUserId";
            var user = new User();
            var model = new AddressModel();
            var cancellationToken = CancellationToken.None;

            _userManagerMock.SetupFindByIdAsync(user);

            var address = new Address();
            _mapperMock.SetupMap(model, address);

            // Act
            await _addressService.AddAsync(userId, model, cancellationToken);

            // Assert
            _addressRepositoryMock.Verify(r => r.AddAsync(address, cancellationToken), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_CallsDeleteAsyncOnRepository_WithSpecifiedId()
        {
            // Arrange
            var id = Guid.NewGuid();
            var cancellationToken = CancellationToken.None;

            // Act
            await _addressService.DeleteAsync(id, cancellationToken);

            // Assert
            _addressRepositoryMock.Verify(r => r.DeleteAsync(id, cancellationToken), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ThrowsEntityNotFoundException_WhenAddressNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();
            var model = new AddressModel();
            var cancellationToken = CancellationToken.None;

            _addressRepositoryMock.SetupGetByIdAsync(null, cancellationToken);

            // Act
            Func<Task> act = async () => await _addressService.UpdateAsync(id, model, cancellationToken);

            // Assert
            await act.Should().ThrowAsync<EntityNotFoundException>();
        }

        [Fact]
        public async Task UpdateAsync_CallsUpdateOnRepository_WithMappedAddress()
        {
            // Arrange
            var id = Guid.NewGuid();
            var address = new Address();
            var model = new AddressModel();
            var cancellationToken = CancellationToken.None;

            _addressRepositoryMock.SetupGetByIdAsync(address, cancellationToken);

            // Act
            await _addressService.UpdateAsync(id, model, cancellationToken);

            // Assert
            _mapperMock.Verify(m => m.Map(model, address), Times.Once);
            _addressRepositoryMock.Verify(r => r.Update(address, cancellationToken), Times.Once);
        }
    }
}
