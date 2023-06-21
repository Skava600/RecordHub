using MassTransit;
using RecordHub.IdentityService.Core.Publishers;
using RecordHub.Shared.Models;

namespace RecordHub.IdentityService.Infrastructure.Publishers
{
    public class SendEmailPublisher : IPublisher<MailData>
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public SendEmailPublisher(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public async Task PublishMessage(MailData mailData)
        {
            await _publishEndpoint.Publish<MailData>(mailData, context =>
            {
                context.SetRoutingKey("send-email");

            });
        }
    }
}
