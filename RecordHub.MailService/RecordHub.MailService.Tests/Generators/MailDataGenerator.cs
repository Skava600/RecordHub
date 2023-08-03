using Bogus;
using RecordHub.Shared.MassTransit.Models;

namespace RecordHub.MailService.Tests.Generators
{
    public class MailDataGenerator : Faker<MailData>
    {
        public MailDataGenerator()
        {
            RuleFor(md => md.From, f => f.Internet.Email());
            RuleFor(md => md.DisplayName, f => f.Name.FullName());
            RuleFor(md => md.ReplyTo, f => f.Internet.Email());
            RuleFor(md => md.ReplyToName, f => f.Name.FullName());
            RuleFor(md => md.Subject, f => f.Lorem.Sentence());
            RuleFor(md => md.Body, f => f.Lorem.Paragraph());
            RuleFor(md => md.To, f => f.Make(f.Random.Number(3, 5), () => f.Internet.Email()));
        }
    }
}
