namespace TaskManager.Application.Common.Interfaces;

public interface ICacheRequestContext
{
  bool HasCacheStatus { get; }
  bool WasCacheHit { get; }
  string? CacheKey { get; }

  void MarkHit(string key);
  void MarkMiss(string key);
}