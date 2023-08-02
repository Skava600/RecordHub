using Bogus;
using RecordHub.MailService.Infrastructure.Settings;

namespace RecordHub.MailService.Tests.Generators
{
    internal class MailSettingsGenerator : Faker<MailSettings>
    {
        public MailSettingsGenerator()
        {
            RuleFor(ms => ms.DisplayName, f => f.Name.FirstName());
            RuleFor(ms => ms.From, f => f.Internet.Email());
            RuleFor(ms => ms.UserName, f => f.Internet.UserName());
            RuleFor(ms => ms.Password, f => f.Internet.Password());
            RuleFor(ms => ms.Host, f => "localhost");
            RuleFor(ms => ms.Port, f => 9009);
            RuleFor(ms => ms.UseSSL, f => false);
            RuleFor(ms => ms.UseStartTls, f => false);
        }
    }
}
