using FluentAssertions;
using Microsoft.Extensions.Options;
using netDumbster.smtp;
using RecordHub.MailService.Tests.Generators;

namespace RecordHub.MailService.Tests.UnitTests.Services
{
    public class MailServiceTests
    {
        [Fact]
        public async Task SendAsync_ValidData_SendsMail()
        {
            // Arrange
            var mailSettings = new MailSettingsGenerator().Generate();
            var server = SimpleSmtpServer.Start(mailSettings.Port);
            var mailService = new Infrastructure.Services.MailService(Options.Create(mailSettings));

            var cancellationToken = CancellationToken.None;
            var mailData = new MailDataGenerator().Generate();

            // Act
            await mailService.SendAsync(mailData, cancellationToken);

            // Assert
            var emails = server.ReceivedEmail;
            emails.Count().Should().Be(1);

            var myMail = emails.First();
            myMail.Subject.Should().BeEquivalentTo(mailData.Subject);

            server.Stop();
        }
    }
}
