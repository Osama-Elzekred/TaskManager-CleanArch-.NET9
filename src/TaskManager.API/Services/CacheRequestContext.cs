using TaskManager.Application.Common.Interfaces;

namespace TaskManager.API.Services;

public sealed class CacheRequestContext : ICacheRequestContext
{
  public bool HasCacheStatus { get; private set; }
  public bool WasCacheHit { get; private set; }
  public string? CacheKey { get; private set; }

  public void MarkHit(string key)
  {
    HasCacheStatus = true;
    WasCacheHit = true;
    CacheKey = key;
  }

  public void MarkMiss(string key)
  {
    HasCacheStatus = true;
    WasCacheHit = false;
    CacheKey = key;
  }
}