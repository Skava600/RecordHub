using RecordHub.IdentityService.Domain.Enum;
using System.Security.Claims;

namespace RecordHub.IdentityService.Tests.Setups
{
    internal static class ClaimsPrincipalMockExtensions
    {
        public static void SetupIsInRoleAsync(this Mock<ClaimsPrincipal> user, bool result)
        {
            user.Setup(u => u.IsInRole(Roles.Admin.ToString())).Returns(true);
        }
    }
}
