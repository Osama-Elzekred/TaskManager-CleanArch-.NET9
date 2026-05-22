namespace TaskManager.Application.Common.Interfaces;

public interface IMetricsService
{
  void IncrementCacheHit();
  void IncrementCacheMiss();
  void IncrementCacheInvalidation();
  long GetCacheHits();
  long GetCacheMisses();
  long GetCacheInvalidations();
}
