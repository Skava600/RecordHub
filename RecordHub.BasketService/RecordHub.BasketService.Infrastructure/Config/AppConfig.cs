namespace RecordHub.BasketService.Infrastructure.Config
{
    public class AppConfig
    {
        public MassTransitOptions MassTransit { get; set; }
        public RedisConfig RedisConfig { get; set; }
        public GrpcConfig GrpcConfig { get; set; }
    }
}
