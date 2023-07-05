using RecordHub.Shared.Models;

namespace RecordHub.MailService.Application.Services
{
    public interface IMailService
    {
        Task<bool> SendAsync(MailData mailData, CancellationToken ct);
    }
}
