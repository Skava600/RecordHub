using Nest;
using RecordHub.CatalogService.Application.DTO;
using System.Collections.ObjectModel;

namespace RecordHub.CatalogService.Tests.Setups
{
    internal static class ElasticClientMockExtensions
    {
        public static void SetupElasticClientMock(this Mock<IElasticClient> elasticClientMock, CancellationToken cancellationToken = default)
        {
            elasticClientMock
                .Setup(ec => ec
                .IndexAsync(
                    It.IsAny<RecordDTO>(),
                    It.IsAny<Func<IndexDescriptor<RecordDTO>, IIndexRequest<RecordDTO>>>(),
                    cancellationToken))
                .Verifiable();
        }

        public static void SetupSearchAsync<T>(this Mock<IElasticClient> elasticClientMock, ReadOnlyCollection<T> expectedDocuments, CancellationToken cancellationToken = default)
            where T : class
        {
            var searchResponseMock = new Mock<ISearchResponse<T>>();
            searchResponseMock.Setup(r => r.IsValid).Returns(true);
            searchResponseMock.Setup(r => r.Documents).Returns(expectedDocuments);

            elasticClientMock
                .Setup(ec => ec.SearchAsync(It.IsAny<Func<SearchDescriptor<T>, ISearchRequest>>(), cancellationToken))
                .ReturnsAsync(searchResponseMock.Object);
        }

        public static void SetupInvalidSearchAsync<T>(this Mock<IElasticClient> elasticClientMock, Exception originalException, CancellationToken cancellationToken)
        where T : class
        {
            var searchResponseMock = new Mock<ISearchResponse<T>>();
            searchResponseMock.Setup(r => r.IsValid).Returns(false);
            searchResponseMock.Setup(r => r.OriginalException).Returns(originalException);

            elasticClientMock.Setup(client => client
                .SearchAsync(
                    It.IsAny<Func<SearchDescriptor<T>, ISearchRequest>>(),
                    cancellationToken))
                .ReturnsAsync(searchResponseMock.Object);
        }

        public static void SetupUpdateAsync<T>(this Mock<IElasticClient> elasticClientMock, UpdateResponse<T> updateResponse, CancellationToken cancellationToken = default)
            where T : class
        {
            elasticClientMock.Setup(client => client
                .UpdateAsync<T>(
                    It.IsAny<DocumentPath<T>>(),
                    It.IsAny<Func<UpdateDescriptor<T, T>, IUpdateRequest<T, T>>>(),
                    cancellationToken))
                .ReturnsAsync(updateResponse)
                .Verifiable();
        }

        public static void SetupDeleteByQueryAsync<T>(this Mock<IElasticClient> elasticClientMock, Func<QueryContainerDescriptor<T>, QueryContainer> querySelector, CancellationToken cancellationToken = default)
            where T : class
        {
            elasticClientMock
                .Setup(ec => ec.DeleteByQueryAsync<T>(It.IsAny<Func<DeleteByQueryDescriptor<T>, IDeleteByQueryRequest>>(), cancellationToken))
                .Verifiable();
        }

        public static void SetupIndexManyAsync<T>(this Mock<IElasticClient> elasticClientMock, IEnumerable<T> documents)
            where T : class
        {
            elasticClientMock
                .Setup(ec => ec.BulkAsync(
                    It.IsAny<BulkRequest>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new BulkResponse())
                .Verifiable();
        }
    }
}
