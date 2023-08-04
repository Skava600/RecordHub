using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RecordHub.IdentityService.Api.Middlewares;
using RecordHub.IdentityService.Core.Exceptions;
using System.Net;

namespace RecordHub.IdentityService.Tests.UnitTests.Middlewares
{
    public class GlobalExceptionHandlingMiddlewareTests
    {
        [Fact]
        public async Task Invoke_ExceptionCaught_ReturnsInternalServerError()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var requestDelegateMock = new Mock<RequestDelegate>();
            var loggerMock = new Mock<ILogger<GlobalExceptionHandlingMiddleware>>();
            var exceptionMessage = "Test exception";

            requestDelegateMock
                .Setup(m => m
                    .Invoke(context))
                    .Throws(new Exception(exceptionMessage));

            var middleware = new GlobalExceptionHandlingMiddleware(requestDelegateMock.Object, loggerMock.Object);

            // Act
            await middleware.Invoke(context);

            // Assert
            var response = context.Response;
            response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
            response.ContentType.Should().Be("application/json");
        }

        [Fact]
        public async Task Invoke_IdentityErrorsExceptionCaught_ReturnsInternalServerErrorWithMessage()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var requestDelegateMock = new Mock<RequestDelegate>();
            var loggerMock = new Mock<ILogger<GlobalExceptionHandlingMiddleware>>();
            var exception = new InvalidCredentialsException("Test IdentityErrorsException");

            requestDelegateMock
                .Setup(m => m
                    .Invoke(context))
                    .Throws(exception);

            var middleware = new GlobalExceptionHandlingMiddleware(requestDelegateMock.Object, loggerMock.Object);

            // Act
            await middleware.Invoke(context);

            // Assert
            var response = context.Response;
            response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
            response.ContentType.Should().Be("application/json");
        }
    }
}
