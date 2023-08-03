using RecordHub.Shared.DTO;
using RecordHub.Shared.Exceptions;
using System.Net;
using System.Text.Json;

namespace RecordHub.ChatService.Api.Middlewares
{
    public class GlobalExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (EntityNotFoundException ex)
            {
                var code = HttpStatusCode.BadRequest;
                await HandleExceptionAsync(context, code, ex);
            }
            catch (Exception ex)
            {
                var code = HttpStatusCode.InternalServerError;
                await HandleExceptionAsync(context, code, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, HttpStatusCode code, Exception ex)
        {
            var response = new BaseResponseDTO
            {
                Errors = new[] { ex.Message },
                IsSuccess = false
            };
            var result = JsonSerializer.Serialize(response);

            _logger.LogWarning("{Exception}: {Message}, Source: {Source}",
                  ex.GetType().Name, ex.Message, ex.Source);

            var httpResponse = context.Response;
            httpResponse.ContentType = "application/json";
            httpResponse.StatusCode = (int)code;

            await httpResponse.WriteAsync(result);
        }
    }
}
