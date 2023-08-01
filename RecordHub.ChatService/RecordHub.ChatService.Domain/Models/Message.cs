namespace RecordHub.ChatService.Domain.Models
{
    public class Message
    {
        public DateTime DateSent { get; set; }
        public string Text { get; set; }
        public string Name { get; set; }

        public Message(string text, string name)
        {
            Text = text;
            Name = name;
            DateSent = DateTime.Now;
        }
    }
}
