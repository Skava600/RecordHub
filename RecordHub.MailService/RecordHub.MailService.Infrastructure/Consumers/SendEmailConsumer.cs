using Hangfire;
using MassTransit;
using RecordHub.MailService.Application.Services;
using RecordHub.Shared.Models;

namespace RecordHub.MailService.Infrastructure.Consumers
{
    public class SendEmailConsumer : IConsumer<MailData>
    {
        private readonly IMailService mailService;
        public SendEmailConsumer(IMailService mailService)
        {
            this.mailService = mailService;
        }

        public Task Consume(ConsumeContext<MailData> context)
        {
            BackgroundJob.Enqueue(
                () => mailService.SendAsync(context.Message, context.CancellationToken));

            return Task.CompletedTask;
        }
    }
}
