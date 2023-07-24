using RecordHub.BasketService.Application.Protos;
using RecordHub.BasketService.Application.Services;
using RecordHub.Shared.Exceptions;

namespace RecordHub.BasketService.Infrastructure.Services
{
    public class CatalogGrpcClient : ICatalogGrpcClient
    {
        private readonly CatalogChecker.CatalogCheckerClient _catalogCheckerClient;

        public CatalogGrpcClient(
           CatalogChecker.CatalogCheckerClient catalogCheckerClient)
        {
            _catalogCheckerClient = catalogCheckerClient;
        }

        public async Task<ProductReply> CheckProductExistenceAsync(string productId, CancellationToken cancellationToken = default)
        {
            var reply = await _catalogCheckerClient.CheckProductExistingAsync(new ProductRequest { ProductId = productId });
            if (!reply.IsExisting)
            {
                throw new EntityNotFoundException(nameof(productId));
            }

            return reply;
        }
    }
}
