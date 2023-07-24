namespace RecordHub.OrderingService.Infrastructure.Config
{
    public class MassTransitOptions
    {
        public string Host { get; set; }
        public string BasketCheckoutQueue { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
