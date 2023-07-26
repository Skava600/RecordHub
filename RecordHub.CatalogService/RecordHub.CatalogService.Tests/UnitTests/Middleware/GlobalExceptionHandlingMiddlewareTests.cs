using FluentAssertions;
using Microsoft.AspNetCore.Http;
using RecordHub.CatalogService.Api.Middlewares;
using RecordHub.Shared.Exceptions;
using System.Net;

namespace RecordHub.CatalogService.Tests.UnitTests.Middleware
{
    public class GlobalExceptionHandlingMiddlewareTests
    {
        [Fact]
        public async Task Invoke_EntityNotFoundException_ReturnsNotFoundResponse()
        {
            // Arrange
            var requestDelegateMock = new Mock<RequestDelegate>();
            requestDelegateMock
                .Setup(m => m.Invoke(It.IsAny<HttpContext>()))
                .Throws(new EntityNotFoundException("Not found"));

            var context = new DefaultHttpContext();
            var response = context.Response;

            var middleware = new GlobalExceptionHandlingMiddleware(requestDelegateMock.Object);

            // Act
            await middleware.Invoke(context);

            // Assert
            response.StatusCode
                .Should()
                .Be((int)HttpStatusCode.NotFound);

            response.ContentType
                .Should()
                .Be("application/json");
        }

        [Fact]
        public async Task Invoke_GenericException_ReturnsInternalServerErrorResponse()
        {
            // Arrange
            var requestDelegateMock = new Mock<RequestDelegate>();
            requestDelegateMock
                .Setup(m => m.Invoke(It.IsAny<HttpContext>()))
                .Throws(new Exception("InternalError"));

            var context = new DefaultHttpContext();
            var response = context.Response;

            var middleware = new GlobalExceptionHandlingMiddleware(requestDelegateMock.Object);

            // Act
            await middleware.Invoke(context);

            // Assert
            response.StatusCode
                .Should()
                .Be((int)HttpStatusCode.InternalServerError);

            response.ContentType
                .Should()
                .Be("application/json");
        }
    }
}
