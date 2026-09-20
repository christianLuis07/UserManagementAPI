using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace UserManagementAPI.Middleware
{
    /// <summary>
    /// Middleware for Bearer token authentication.
    /// Validates the Authorization header and grants access only to requests with a valid token.
    /// </summary>
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AuthenticationMiddleware> _logger;

        // Endpoints that do not require authentication
        private static readonly HashSet<string> PublicEndpoints = new(StringComparer.OrdinalIgnoreCase)
        {
            "/api/health",
            "/openapi/v1.json"
        };

        // Valid tokens for testing (use a database or JWT in production)
        private static readonly Dictionary<string, string> ValidTokens = new()
        {
            { "token-techhive-2024", "TechHive Admin" },
            { "token-hr-department", "HR Department" },
            { "token-it-department", "IT Department" }
        };

        public AuthenticationMiddleware(RequestDelegate next, ILogger<AuthenticationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value ?? string.Empty;

            // Skip authentication for public endpoints
            if (IsPublicEndpoint(path))
            {
                _logger.LogInformation("✓ Public endpoint accessed: {Path}", path);
                await _next(context);
                return;
            }

            var authHeader = context.Request.Headers["Authorization"].ToString();

            if (string.IsNullOrEmpty(authHeader))
            {
                _logger.LogWarning("✗ Missing Authorization header for: {Path}", path);
                await RespondUnauthorized(context, "Authorization header tidak ditemukan.");
                return;
            }

            if (!authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("✗ Invalid Authorization format for: {Path}", path);
                await RespondUnauthorized(context, "Format Authorization harus 'Bearer <token>'.");
                return;
            }

            var token = authHeader.Substring("Bearer ".Length).Trim();

            if (!ValidTokens.TryGetValue(token, out var userName))
            {
                _logger.LogWarning("✗ Invalid or expired token for: {Path}", path);
                await RespondUnauthorized(context, "Token tidak valid atau telah kadaluarsa.");
                return;
            }

            _logger.LogInformation("✓ Authentication successful for: {UserName} on {Path}", userName, path);
            context.Items["User"] = userName;
            context.Items["Token"] = token;

            await _next(context);
        }

        private bool IsPublicEndpoint(string path)
        {
            foreach (var publicEndpoint in PublicEndpoints)
            {
                if (path.Equals(publicEndpoint, StringComparison.OrdinalIgnoreCase) ||
                    path.StartsWith(publicEndpoint, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private static Task RespondUnauthorized(HttpContext context, string message)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;

            var errorResponse = new
            {
                statusCode = 401,
                message = message,
                timestamp = DateTime.UtcNow
            };

            var json = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            return context.Response.WriteAsync(json);
        }
    }
}
