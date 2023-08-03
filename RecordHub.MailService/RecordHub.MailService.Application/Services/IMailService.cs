using RecordHub.Shared.MassTransit.Models;

namespace RecordHub.MailService.Application.Services
{
    public interface IMailService
    {
        Task SendAsync(MailData mailData, CancellationToken ct);
    }
}
