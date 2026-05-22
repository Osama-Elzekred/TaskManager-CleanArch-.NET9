namespace TaskManager.Application.Common.Interfaces;

public interface ICacheableRequest<TResponse>
{
  string GetCacheKey(Guid userId);
}