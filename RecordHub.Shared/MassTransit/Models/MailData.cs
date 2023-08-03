namespace RecordHub.Shared.MassTransit.Models
{
    public class MailData
    {

        // Receiver
        public List<string> To { get; set; }

        // Sender
        public string? From { get; set; }

        public string? DisplayName { get; set; }

        public string? ReplyTo { get; set; }

        public string? ReplyToName { get; set; }

        // Content
        public string Subject { get; set; }

        public string? Body { get; set; }

        public MailData(
            List<string> to,
            string subject,
            string? body = null,
            string? from = null,
            string? displayName = null,
            string? replyTo = null,
            string? replyToName = null)
        {
            // Receiver
            To = to;
            // Sender
            From = from;
            DisplayName = displayName;
            ReplyTo = replyTo;
            ReplyToName = replyToName;

            // Content
            Subject = subject;
            Body = body;
        }

        public MailData() { }
    }
}
