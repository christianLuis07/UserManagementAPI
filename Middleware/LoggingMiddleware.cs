using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace UserManagementAPI.Middleware
{
    /// <summary>
    /// Middleware untuk mencatat semua permintaan HTTP dan respons keluar
    /// </summary>
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LoggingMiddleware> _logger;

        public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            
            var requestLog = new
            {
                Timestamp = DateTime.UtcNow,
                Method = context.Request.Method,
                Path = context.Request.Path.Value,
                QueryString = context.Request.QueryString.Value,
                RemoteIP = context.Connection.RemoteIpAddress?.ToString()
            };

            _logger.LogInformation(
                "→ INBOUND REQUEST: {Method} {Path} dari {RemoteIP}",
                requestLog.Method,
                requestLog.Path,
                requestLog.RemoteIP
            );

            var originalBodyStream = context.Response.Body;

            try
            {
                using (var responseStream = new MemoryStream())
                {
                    context.Response.Body = responseStream;

                    await _next(context);

                    stopwatch.Stop();
                    var elapsedMs = stopwatch.ElapsedMilliseconds;

                    _logger.LogInformation(
                        "← OUTBOUND RESPONSE: {StatusCode} {Path} | Waktu: {ElapsedMs}ms",
                        context.Response.StatusCode,
                        requestLog.Path,
                        elapsedMs
                    );

                    await responseStream.CopyToAsync(originalBodyStream);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "✗ ERROR during request: {Method} {Path} - {Message}",
                    requestLog.Method,
                    requestLog.Path,
                    ex.Message
                );
                throw;
            }
            finally
            {
                context.Response.Body = originalBodyStream;
            }
        }
    }
}
