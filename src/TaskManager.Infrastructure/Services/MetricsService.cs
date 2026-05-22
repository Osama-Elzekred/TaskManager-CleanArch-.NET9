using TaskManager.Application.Common.Interfaces;

namespace TaskManager.Infrastructure.Services;

public class MetricsService : IMetricsService
{
  private long _cacheHits;
  private long _cacheMisses;
  private long _cacheInvalidations;

  public void IncrementCacheHit() => Interlocked.Increment(ref _cacheHits);
  public void IncrementCacheMiss() => Interlocked.Increment(ref _cacheMisses);
  public void IncrementCacheInvalidation() => Interlocked.Increment(ref _cacheInvalidations);

  public long GetCacheHits() => Interlocked.Read(ref _cacheHits);
  public long GetCacheMisses() => Interlocked.Read(ref _cacheMisses);
  public long GetCacheInvalidations() => Interlocked.Read(ref _cacheInvalidations);
}
