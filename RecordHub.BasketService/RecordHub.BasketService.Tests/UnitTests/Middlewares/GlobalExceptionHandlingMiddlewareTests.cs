using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RecordHub.BasketService.Api.Middlewares;
using RecordHub.BasketService.Application.Exceptions;
using RecordHub.Shared.DTO;
using RecordHub.Shared.Exceptions;
using System.Net;
using System.Text;
using System.Text.Json;

namespace RecordHub.BasketService.Tests.UnitTests.Middlewares
{
    public class GlobalExceptionHandlingMiddlewareTests
    {
        [Fact]
        public async Task Invoke_InvalidRequestBodyException_ReturnsBadRequest()
        {
            // Arrange
            var mockNext = new Mock<RequestDelegate>();
            var mockLogger = new Mock<ILogger<GlobalExceptionHandlingMiddleware>>();
            var context = CreateMockHttpContext();
            var middleware = new GlobalExceptionHandlingMiddleware(mockNext.Object, mockLogger.Object);

            var errorMessage = "Invalid request body.";
            mockNext.Setup(next => next(context)).Throws(new InvalidRequestBodyException { Errors = new List<string> { errorMessage } });

            // Act
            await middleware.Invoke(context);

            // Assert
            context.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);

            var responseBody = await GetResponseBody(context.Response);
            var responseDTO = JsonSerializer.Deserialize<BaseResponseDTO>(responseBody);

            responseDTO.Should().NotBeNull();
            responseDTO.IsSuccess.Should().BeFalse();
            responseDTO.Errors.Should().ContainSingle().And.Contain(errorMessage);
        }

        [Fact]
        public async Task Invoke_BasketIsEmptyException_ReturnsBadRequest()
        {
            // Arrange
            var mockNext = new Mock<RequestDelegate>();
            var mockLogger = new Mock<ILogger<GlobalExceptionHandlingMiddleware>>();
            var context = CreateMockHttpContext();
            var middleware = new GlobalExceptionHandlingMiddleware(mockNext.Object, mockLogger.Object);

            mockNext.Setup(next => next(context)).Throws(new BasketIsEmptyException());

            // Act
            await middleware.Invoke(context);

            // Assert
            context.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);

            var responseBody = await GetResponseBody(context.Response);
            var responseDTO = JsonSerializer.Deserialize<BaseResponseDTO>(responseBody);

            responseDTO.Should().NotBeNull();
            responseDTO.IsSuccess.Should().BeFalse();
            responseDTO.Errors.Should().ContainSingle().And.Contain(BasketIsEmptyException.Message);
        }

        [Fact]
        public async Task Invoke_ItemMissingInBasketException_ReturnsBadRequest()
        {
            // Arrange
            var mockNext = new Mock<RequestDelegate>();
            var mockLogger = new Mock<ILogger<GlobalExceptionHandlingMiddleware>>();
            var context = CreateMockHttpContext();
            var middleware = new GlobalExceptionHandlingMiddleware(mockNext.Object, mockLogger.Object);

            mockNext.Setup(next => next(context)).Throws(new ItemMissingInBasketException());

            // Act
            await middleware.Invoke(context);

            // Assert
            context.Response.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);

            var responseBody = await GetResponseBody(context.Response);
            var responseDTO = JsonSerializer.Deserialize<BaseResponseDTO>(responseBody);

            responseDTO.Should().NotBeNull();
            responseDTO.IsSuccess.Should().BeFalse();
            responseDTO.Errors.Should().ContainSingle().And.Contain(ItemMissingInBasketException.Message);
        }

        [Fact]
        public async Task Invoke_UnhandledException_ReturnsInternalServerError()
        {
            // Arrange
            var mockNext = new Mock<RequestDelegate>();
            var mockLogger = new Mock<ILogger<GlobalExceptionHandlingMiddleware>>();
            var context = CreateMockHttpContext();
            var middleware = new GlobalExceptionHandlingMiddleware(mockNext.Object, mockLogger.Object);

            var errorMessage = "An unhandled exception occurred.";
            mockNext.Setup(next => next(context)).Throws(new Exception(errorMessage));

            // Act
            await middleware.Invoke(context);

            // Assert
            context.Response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);

            var responseBody = await GetResponseBody(context.Response);
            var responseDTO = JsonSerializer.Deserialize<BaseResponseDTO>(responseBody);

            responseDTO.Should().NotBeNull();
            responseDTO.IsSuccess.Should().BeFalse();
            responseDTO.Errors.Should().ContainSingle().And.Contain(errorMessage);
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
