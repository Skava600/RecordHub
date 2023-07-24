using FluentAssertions;
using RecordHub.IdentityService.Core.Services;
using RecordHub.IdentityService.Domain.Data.Entities;
using RecordHub.IdentityService.Domain.Enum;
using System.Security.Claims;

namespace RecordHub.IdentityService.Tests.Setups
{
    internal static class ITokenServiceMockExtensions
    {
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
    }
}
