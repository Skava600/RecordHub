using RecordHub.MailService.Domain.Models;

namespace RecordHub.MailService.Application.Services
{
    public interface IMailService
    {
        Task<bool> SendAsync(MailData mailData, CancellationToken ct);
    }
}
