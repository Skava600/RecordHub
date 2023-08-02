using Hangfire;
using MassTransit;
using RecordHub.MailService.Application.Services;
using RecordHub.Shared.MassTransit.Models;

namespace RecordHub.MailService.Infrastructure.Consumers
{
    public class SendEmailConsumer : IConsumer<MailData>
    {
        private readonly IMailService _mailService;

        public SendEmailConsumer(IMailService mailService)
        {
            this._mailService = mailService;
        }

        public Task Consume(ConsumeContext<MailData> context)
        {
            BackgroundJob.Enqueue(
                () => _mailService.SendAsync(context.Message, context.CancellationToken));

            return Task.CompletedTask;
        }
    }
}
