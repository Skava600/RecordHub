using RecordHub.IdentityService.Core.DTO;
using RecordHub.IdentityService.Core.Services;
using RecordHub.IdentityService.Domain.Models;

namespace RecordHub.IdentityService.Tests.Controllers
{
    public static class AccountControllerTestsSetup
    {
        public static void SetupLoginAsync(this Mock<IAccountService> accountServiceMock, string expectedToken)
        {
            accountServiceMock
                .Setup(m => m.LoginAsync(It.IsAny<LoginModel>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedToken);
        }

        public static void SetupRegisterAsync(this Mock<IAccountService> accountServiceMock)
        {
            accountServiceMock
                .Setup(m => m.RegisterAsync(It.IsAny<RegisterModel>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
        }

        public static void SetupGetUserInfoAsync(this Mock<IAccountService> accountServiceMock, UserDTO expectedUser)
        {
            accountServiceMock
                .Setup(m => m.GetUserInfoAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedUser);
        }

        public static void SetupVerifyEmailAsync(this Mock<IAccountService> accountServiceMock)
        {
            accountServiceMock
                .Setup(m => m.VerifyEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
        }

        public static void SetupSendEmailVerificationAsync(this Mock<IAccountService> accountServiceMock)
        {
            accountServiceMock
                .Setup(m => m.SendEmailVerificationAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
        }
    }
}
