using Grpc.Core;
using Microsoft.Extensions.Logging;
using RecordHub.CatalogService.Application.Data;

namespace RecordHub.CatalogService.Infrastructure.Services
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
            bool isExisting = true;
            if (record != null)
            {
                return new ProductReply { IsExisting = isExisting, Name = record.Name, Price = record.Price };
            }

            isExisting = false;

            return new ProductReply { IsExisting = isExisting };
        }
    }
}
