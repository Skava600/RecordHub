using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RecordHub.OrderingService.Api.Middlewares;
using System.Net;
using System.Text;
using System.Text.Json;

namespace RecordHub.OrderingService.Tests.UnitTests.Middlewares
{
    public class GlobalExceptionHandlingMiddlewareTests
    {
        [Fact]
        public async Task Invoke_Exception_InternalServerError()
        {
            // Arrange
            var mockNext = new Mock<RequestDelegate>();
            var mockLogger = new Mock<ILogger<GlobalExceptionHandlingMiddleware>>();
            var context = CreateMockHttpContext();
            var middleware = new GlobalExceptionHandlingMiddleware(mockNext.Object, mockLogger.Object);

            var exceptionMessage = "Some error occurred.";
            mockNext.Setup(next => next(context)).Throws(new Exception(exceptionMessage));

            // Act
            await middleware.Invoke(context);

            // Assert
            context.Response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);

            var responseBody = await GetResponseBody(context.Response);
            responseBody.Should().Be(JsonSerializer.Serialize(exceptionMessage));
        }

        private HttpContext CreateMockHttpContext()
        {
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();
            return context;
        }

        private async Task<string> GetResponseBody(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(response.Body, Encoding.UTF8);
            return await reader.ReadToEndAsync();
        }
    }
}