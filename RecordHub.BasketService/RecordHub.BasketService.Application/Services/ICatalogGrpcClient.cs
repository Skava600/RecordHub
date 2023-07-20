using RecordHub.BasketService.Application.Protos;

namespace RecordHub.BasketService.Application.Services
{
    public interface ICatalogGrpcClient
    {
        Task<ProductReply> CheckProductExistenceAsync(
            string productId,
            CancellationToken cancellationToken = default);
    }
}
