﻿using RecordHub.IdentityService.Core.Exceptions;
using System.Net;
using System.Text.Json;

namespace RecordHub.IdentityService.Api.Middlewares
{
    public class GlobalExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (MultipleErrorException ex)
            {
                var code = HttpStatusCode.InternalServerError;
                var message = ex.Message + string.Join("\n", ex.Errors.Select(e => $"{e.Code}: {e.Description}"));


                await HandleExceptionAsync(context, code, message);
            }
            catch (Exception ex)
            {
                var code = HttpStatusCode.InternalServerError;
                var message = ex.Message;

                await HandleExceptionAsync(context, code, message);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, HttpStatusCode code, string message)
        {
            var result = JsonSerializer.Serialize(message);

            var httpResponse = context.Response;
            httpResponse.ContentType = "application/json";
            httpResponse.StatusCode = (int)code;

            await httpResponse.WriteAsync(result);
        }
    }
}
