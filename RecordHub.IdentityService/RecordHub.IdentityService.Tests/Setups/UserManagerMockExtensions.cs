using Microsoft.AspNetCore.Identity;
using RecordHub.IdentityService.Domain.Data.Entities;

namespace RecordHub.IdentityService.Tests.Setups
{
    internal static class UserManagerMockExtensions
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

        public static void SetupFindByIdAsync(this Mock<UserManager<User>> userManagerMock, User? user)
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
    }
}
