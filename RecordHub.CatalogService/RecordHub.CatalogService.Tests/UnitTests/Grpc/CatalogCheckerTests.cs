using FluentAssertions;
using Microsoft.Extensions.Logging;
using RecordHub.CatalogService.Api.GrpcServices;
using RecordHub.CatalogService.Api.Protos;
using RecordHub.CatalogService.Application.Data;
using RecordHub.CatalogService.Tests.Generators;
using RecordHub.CatalogService.Tests.UnitTests.Grpc.Helpers;

namespace RecordHub.CatalogService.Tests.UnitTests.Grpc
{

    public class CatalogCheckerTests
    {
        private readonly RecordGenerator _recordGenerator;

        public CatalogCheckerTests()
        {
            _recordGenerator = new RecordGenerator();
        }

        [Fact]
        public async Task CheckProductExisting_ValidId_ReturnProductReply()
        {
            CancellationToken cancellationToken = CancellationToken.None;
            var id = Guid.NewGuid();

            // Arrange
            var unitOfWork = new Mock<IUnitOfWork>();
            var record = _recordGenerator.GenerateRecord();

            var productReply = new ProductReply
            {
                IsExisting = true,
                Name = record.Name,
                Price = record.Price,
            };

            unitOfWork.Setup(
                 m => m.Records.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(record);

            var service = new CatalogCheckerService(new Mock<ILogger<CatalogCheckerService>>().Object, unitOfWork.Object);

            // Act
            var response = await service.CheckProductExisting(
                new ProductRequest { ProductId = id.ToString() }, TestServerCallContext.Create());

            // Assert
            unitOfWork.Verify(v => v.Records.GetByIdAsync(id, cancellationToken));
            response.Should().BeEquivalentTo(response);
        }
    }
}
