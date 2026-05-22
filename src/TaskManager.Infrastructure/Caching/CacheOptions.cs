namespace TaskManager.Infrastructure.Caching;

public class CacheOptions
{
  // default TTL in seconds
  public int DefaultTtlSeconds { get; set; } = 300; // 5 minutes
}
