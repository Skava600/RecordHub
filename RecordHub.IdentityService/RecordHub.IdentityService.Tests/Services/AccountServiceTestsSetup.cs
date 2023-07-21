﻿using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using RecordHub.IdentityService.Core.DTO;
using RecordHub.IdentityService.Core.Publishers;
using RecordHub.IdentityService.Core.Services;
using RecordHub.IdentityService.Domain.Data.Entities;
using RecordHub.IdentityService.Domain.Enum;
using RecordHub.IdentityService.Persistence.Data.Repositories.Generic;
using RecordHub.Shared.MassTransit.Models;
using System.Security.Claims;

namespace RecordHub.IdentityService.Tests.Services
{
    public static class AccountServiceTestsSetup
    {
        public static void SetupCreateAsync(this Mock<UserManager<User>> userManagerMock, IdentityResult creatingUserResult = null)
        {
            creatingUserResult ??= IdentityResult.Success;

            userManagerMock
                .Setup(m => m.CreateAsync(
                    It.IsAny<User>(),
                    It.IsAny<string>()))
                .ReturnsAsync(() => creatingUserResult);
        }

        public static void SetupAddToRoleAsync(this Mock<UserManager<User>> userManagerMock, IdentityResult addingToRoleResult = null)
        {
            addingToRoleResult ??= IdentityResult.Success;

            userManagerMock
                .Setup(m => m.AddToRoleAsync(
                    It.IsAny<User>(),
                    It.IsAny<string>()))
                .ReturnsAsync(() => addingToRoleResult);
        }

        public static void SetupGetRolesAsync(this Mock<UserManager<User>> userManager, List<string> roles)
        {
            userManager
                .Setup(m => m.GetRolesAsync(It.IsAny<User>()))
                .ReturnsAsync(roles);
        }

        public static void SetupFindByNameAsync(this Mock<UserManager<User>> userManager, User user)
        {
            userManager
                .Setup(m => m.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(user);
        }

        public static void SetupCheckPasswordAsync(this Mock<UserManager<User>> userManager, bool result)
        {
            userManager
                .Setup(m => m.CheckPasswordAsync(
                    It.IsAny<User>(),
                    It.IsAny<string>()))
                .ReturnsAsync(result);
        }

        public static void SetupGenerateJwtToken(this Mock<ITokenService> tokenService, string token, string role = nameof(Roles.User))
        {
            tokenService
                .Setup(m => m.GenerateJwtToken(
                    It.IsAny<User>(),
                    It.IsAny<List<Claim>>()))
                .Returns(token)
                .Callback<User, IEnumerable<Claim>>((_, claims) =>
                {
                    var roleClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
                    roleClaim.Should().NotBeNull();
                    role.Should().Be(roleClaim.Value);
                });
        }

        public static void SetupMapUserToUserDTO(this Mock<IMapper> mapperMock, User user, UserDTO userDTO)
        {
            mapperMock
                .Setup(m => m.Map<UserDTO>(It.IsAny<User>()))
                .Returns(userDTO);
        }

        public static void SetupGetAddressesByUserId(this Mock<IAddressRepository> addressRepositoryMock, IEnumerable<Address> addresses)
        {
            addressRepositoryMock
                .Setup(m => m.GetAddressesByUserId(It.IsAny<Guid>()))
                .Returns(addresses);
        }

        public static void SetupFindByIdAsync(this Mock<UserManager<User>> userManagerMock, User user)
        {
            userManagerMock
                .Setup(m => m.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(user);
        }

        public static void SetupConfirmEmailAsync(this Mock<UserManager<User>> userManagerMock, IdentityResult result)
        {
            userManagerMock
                .Setup(m => m.ConfirmEmailAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(result);
        }

        public static void SetupGenerateEmailConfirmationTokenAsync(this Mock<UserManager<User>> userManagerMock, string token)
        {
            userManagerMock
                .Setup(m => m.GenerateEmailConfirmationTokenAsync(It.IsAny<User>()))
                .ReturnsAsync(token);
        }

        public static void SetupPublishMessage(this Mock<IPublisher<MailData>> mailPublisherMock, Task completedTask)
        {
            mailPublisherMock
                .Setup(m => m.PublishMessage(It.IsAny<MailData>()))
                .Returns(completedTask);
        }
    }
}
