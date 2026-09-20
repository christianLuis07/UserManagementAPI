using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace UserManagementAPI.Middleware
{
    /// <summary>
    /// Middleware that handles errors and exceptions centrally.
    /// Ensures all errors are returned in a consistent JSON format.
    /// </summary>
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";
            var response = context.Response;

            var errorResponse = new ErrorResponse();

            switch (ex)
            {
                case ArgumentNullException argEx:
                    response.StatusCode = (int)HttpStatusCode.BadRequest; // 400
                    errorResponse.StatusCode = response.StatusCode;
                    errorResponse.Message = "Data yang dikirim tidak lengkap atau tidak valid.";
                    errorResponse.Details = argEx.Message;
                    break;

                case UnauthorizedAccessException unAuthEx:
                    response.StatusCode = (int)HttpStatusCode.Unauthorized; // 401
                    errorResponse.StatusCode = response.StatusCode;
                    errorResponse.Message = "Anda tidak memiliki akses ke resource ini.";
                    errorResponse.Details = unAuthEx.Message;
                    break;

                case KeyNotFoundException keyEx:
                    response.StatusCode = (int)HttpStatusCode.NotFound; // 404
                    errorResponse.StatusCode = response.StatusCode;
                    errorResponse.Message = "Resource tidak ditemukan.";
                    errorResponse.Details = keyEx.Message;
                    break;

                case InvalidOperationException invalidEx:
                    response.StatusCode = (int)HttpStatusCode.BadRequest; // 400
                    errorResponse.StatusCode = response.StatusCode;
                    errorResponse.Message = "Operasi tidak dapat dilakukan pada saat ini.";
                    errorResponse.Details = invalidEx.Message;
                    break;

                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError; // 500
                    errorResponse.StatusCode = response.StatusCode;
                    errorResponse.Message = "Terjadi kesalahan internal pada server.";
                    errorResponse.Details = ex.Message;
                    break;
            }

            errorResponse.Timestamp = DateTime.UtcNow;

            var result = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            return context.Response.WriteAsync(result);
        }
    }

    /// <summary>
    /// Model for a consistent error response payload.
    /// </summary>
    public class ErrorResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? Details { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
