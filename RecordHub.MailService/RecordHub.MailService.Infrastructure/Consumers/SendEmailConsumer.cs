using RecordHub.MailService.Application.Services;
using RecordHub.MailService.Domain.Models;

namespace RecordHub.MailService.Infrastructure.Consumers
{
    public class SendEmailConsumer : IConsumer<MailData>
    {
        private readonly IMailService mailService;
        public SendEmailConsumer(IMailService mailService)
        {
            this.mailService = mailService;
        }

        public async Task Consume(MassTransit.ConsumeContext<MailData> context)
        {
            await mailService.SendAsync(context.Message, context.CancellationToken);
        }
    }
}
