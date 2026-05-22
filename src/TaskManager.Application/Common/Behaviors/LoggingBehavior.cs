using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace TaskManager.Application.Common.Behaviors;

/// <summary>
/// CQRS pipeline: logs MediatR request name and duration.
/// </summary>
public sealed class LoggingBehavior<TRequest, TResponse>(
    ILogger<LoggingBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var stopwatch = Stopwatch.StartNew();

        logger.LogDebug("Handling {RequestName}", requestName);

        var response = await next();

        stopwatch.Stop();
        logger.LogDebug("{RequestName} completed in {ElapsedMs}ms", requestName, stopwatch.ElapsedMilliseconds);

        return response;
    }
}
