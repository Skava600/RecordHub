namespace RecordHub.OrderingService.Infrastructure.Config
{
    public class MassTransitOptions
    {
        public string Host { get; set; }
        public string BasketCheckoutQueue { get; set; }
    }
}
