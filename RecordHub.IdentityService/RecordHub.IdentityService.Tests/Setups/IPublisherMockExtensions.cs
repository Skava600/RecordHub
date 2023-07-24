using RecordHub.IdentityService.Core.Publishers;
using RecordHub.Shared.MassTransit.Models;

namespace RecordHub.IdentityService.Tests.Setups
{
    internal static class IPublisherMockExtensions
    {
        public static void SetupPublishMessage(this Mock<IPublisher<MailData>> mailPublisherMock, Task completedTask)
        {
            mailPublisherMock
                .Setup(m => m.PublishMessage(It.IsAny<MailData>()))
                .Returns(completedTask);
        }
    }
}
