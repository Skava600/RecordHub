using Grpc.Core;
using RecordHub.CatalogService.Api.Protos;
using RecordHub.CatalogService.Application.Data;
namespace RecordHub.CatalogService.Api.GrpcServices
{
    public class CatalogCheckerService : CatalogChecker.CatalogCheckerBase
    {
        private readonly ILogger<CatalogCheckerService> _logger;
        private readonly IUnitOfWork _repository;

        public CatalogCheckerService(ILogger<CatalogCheckerService> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _repository = unitOfWork;
        }

        public override async Task<ProductReply> CheckProductExisting(ProductRequest request, ServerCallContext context)
        {
            var record = await _repository.Records.GetByIdAsync(Guid.Parse(request.ProductId));
            bool isExisting = record != null;

            _logger.LogInformation($"Grpc call existing of product {request.ProductId}, Existing - {isExisting}");

            return new ProductReply { IsExisting = isExisting, Name = record?.Name ?? null, Price = record?.Price ?? 0 };
        }
    }
}
