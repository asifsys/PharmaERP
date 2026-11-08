using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace PharmaERP.Observability;

/// <summary>
/// Assigns a unique identifier to each HTTP request.
/// The same identifier can be used to trace a request
/// across the ERP microservices.
/// </summary>
public sealed class CorrelationIdMiddleware
{
    private const string HeaderName = "X-Correlation-ID";

    private readonly RequestDelegate _next;
    private readonly ILogger<CorrelationIdMiddleware> _logger;

    public CorrelationIdMiddleware(
        RequestDelegate next,
        ILogger<CorrelationIdMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Reuse a valid incoming correlation ID.
        // Generate a new ID if none is supplied or it is invalid.
        string correlationId =
            context.Request.Headers[HeaderName].FirstOrDefault()
            is string incomingId &&
            Guid.TryParse(incomingId, out var parsedId)
                ? parsedId.ToString()
                : Guid.NewGuid().ToString();

        // Make the ID available in the response.
        context.Response.Headers[HeaderName] = correlationId;

        // Include it in structured logging.
        using (_logger.BeginScope(
            new Dictionary<string, object>
            {
                ["CorrelationId"] = correlationId
            }))
        {
            await _next(context);
        }
    }
}