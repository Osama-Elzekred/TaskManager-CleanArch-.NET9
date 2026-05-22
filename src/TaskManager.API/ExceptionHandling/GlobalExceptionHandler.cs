using System.Net;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using TaskManager.Application.Common;
using TaskManager.Application.Common.Exceptions;

namespace TaskManager.API.ExceptionHandling;

/// <summary>
/// Maps unhandled exceptions to <see cref="ApiResponse{T}"/> (bonus: generic response wrapper).
/// </summary>
public sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger,
    IHostEnvironment environment) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is ValidationException validationException)
        {
            logger.LogDebug("Validation failed for {Path}", httpContext.Request.Path);
            await WriteResponseAsync(
                httpContext,
                StatusCodes.Status400BadRequest,
                ApiResponse<object>.ValidationErrorResponse(
                    validationException.Errors.Select(e => e.ErrorMessage).ToList()),
                cancellationToken);
            return true;
        }

        var (statusCode, response) = MapException(exception);

        if (statusCode >= 500)
            logger.LogError(exception, "Unhandled exception at {Path}", httpContext.Request.Path);
        else
            logger.LogDebug(exception, "Client error at {Path}", httpContext.Request.Path);

        await WriteResponseAsync(httpContext, statusCode, response, cancellationToken);
        return true;
    }

    private (int StatusCode, ApiResponse<object> Response) MapException(Exception exception)
    {
        return exception switch
        {
            NotFoundException ex => (
                StatusCodes.Status404NotFound,
                ApiResponse<object>.ErrorResponse(ex.Message)),

            UnauthorizedAccessException => (
                StatusCodes.Status401Unauthorized,
                ApiResponse<object>.ErrorResponse("Unauthorized")),

            InvalidOperationException ex => (
                StatusCodes.Status400BadRequest,
                ApiResponse<object>.ErrorResponse(ex.Message)),

            _ => (
                StatusCodes.Status500InternalServerError,
                ApiResponse<object>.ErrorResponse(
                    environment.IsDevelopment()
                        ? exception.Message
                        : "An unexpected error occurred"))
        };
    }

    private static Task WriteResponseAsync(
        HttpContext httpContext,
        int statusCode,
        ApiResponse<object> response,
        CancellationToken cancellationToken)
    {
        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/json";
        return httpContext.Response.WriteAsJsonAsync(response, cancellationToken);
    }
}
