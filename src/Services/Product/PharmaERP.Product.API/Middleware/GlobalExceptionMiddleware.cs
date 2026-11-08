using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using PharmaERP.Product.API.Services;

namespace PharmaERP.Product.API.Middleware;

public sealed class GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled API exception. CorrelationId: {CorrelationId}", context.TraceIdentifier);
            if (context.Response.HasStarted) throw;

            var (status, title) = ex switch
            {
                ValidationException => (StatusCodes.Status400BadRequest, "Validation failed"),
                ConflictException => (StatusCodes.Status409Conflict, "Conflict"),
                DbUpdateException => (StatusCodes.Status409Conflict, "Database constraint conflict"),
                _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred")
            };

            context.Response.Clear();
            context.Response.StatusCode = status;
            context.Response.ContentType = "application/problem+json";
            await context.Response.WriteAsJsonAsync(new
            {
                type = "about:blank",
                title,
                status,
                detail = status < 500 ? ex.Message : "Please contact support with the correlation ID.",
                correlationId = context.TraceIdentifier
            });
        }
    }
}
