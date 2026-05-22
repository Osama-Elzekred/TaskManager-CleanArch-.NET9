using System.Diagnostics;

namespace TaskManager.API.Middleware;

public sealed class RequestLoggingMiddleware
{
  private readonly RequestDelegate _next;
  private readonly ILogger<RequestLoggingMiddleware> _logger;

  public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
  {
    _next = next;
    _logger = logger;
  }

  public async Task InvokeAsync(HttpContext context)
  {
    var stopwatch = Stopwatch.StartNew();

    context.Response.OnCompleted(() =>
    {
      stopwatch.Stop();
      _logger.LogInformation(
          "HTTP {Method} {Path} responded {StatusCode} in {ElapsedMs}ms",
          context.Request.Method,
          context.Request.Path.Value,
          context.Response.StatusCode,
          stopwatch.ElapsedMilliseconds);

      return Task.CompletedTask;
    });

    await _next(context);
  }
}