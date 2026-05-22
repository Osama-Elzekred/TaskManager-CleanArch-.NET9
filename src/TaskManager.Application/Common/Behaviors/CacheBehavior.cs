using MediatR;
using TaskManager.Application.Common.Interfaces;

namespace TaskManager.Application.Common.Behaviors;

public sealed class CacheBehavior<TRequest, TResponse>(
  ICacheService cacheService,
  ICacheRequestContext cacheRequestContext,
  ICurrentUserService currentUserService,
  IMetricsService metricsService) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
  public async Task<TResponse> Handle(
      TRequest request,
      RequestHandlerDelegate<TResponse> next,
      CancellationToken cancellationToken)
  {
    if (request is not ICacheableRequest<TResponse> cacheableRequest)
    {
      return await next();
    }

    var cacheKey = cacheableRequest.GetCacheKey(currentUserService.UserId);
    var cached = await cacheService.GetAsync<TResponse>(cacheKey, cancellationToken);
    if (cached is not null)
    {
      cacheRequestContext.MarkHit(cacheKey);
      metricsService.IncrementCacheHit();
      return cached;
    }

    var response = await next();

    await cacheService.SetAsync(cacheKey, response, cancellationToken: cancellationToken);
    cacheRequestContext.MarkMiss(cacheKey);
    metricsService.IncrementCacheMiss();

    return response;
  }
}