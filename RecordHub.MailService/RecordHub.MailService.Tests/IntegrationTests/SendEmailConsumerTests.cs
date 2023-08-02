using FluentAssertions;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using netDumbster.smtp;
using RecordHub.MailService.Tests.Generators;
using RecordHub.MailService.Tests.IntegrationTests.Helpers;
using RecordHub.Shared.MassTransit.Models;

namespace RecordHub.MailService.Tests.IntegrationTests
{
    public class SendEmailConsumerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;

        public SendEmailConsumerTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task SendEmailConsumer_Should_InvokeMailServiceSendAsync()
        {
            // Arrange
            var server = SimpleSmtpServer.Start(9009);
            var harness = _factory.Services.GetRequiredService<ITestHarness>();

            var mailData = new MailDataGenerator().Generate();

            // Act
            await harness.Bus.Publish(mailData);

            // Assert
            (await harness.Consumed.Any<MailData>()).Should().BeTrue();
            (await harness.Published.Any<MailData>()).Should().BeTrue();

            await Task.Delay(TimeSpan.FromSeconds(1));

            var emails = server.ReceivedEmail;
            emails.Count().Should().Be(1);

            var myMail = emails.First();
            myMail.Subject.Should().BeEquivalentTo(mailData.Subject);

            server.Stop();
            await harness.Stop();
        }
    }
}
