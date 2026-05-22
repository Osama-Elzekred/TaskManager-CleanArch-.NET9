using Microsoft.Extensions.Logging;
using TaskManager.Application.Common.Interfaces;

namespace TaskManager.Application.Common;

public class ProjectCacheInvalidator
{
    private readonly ICacheService _cacheService;
    private readonly IMetricsService _metrics;
    private readonly ILogger<ProjectCacheInvalidator> _logger;

    public ProjectCacheInvalidator(ICacheService cacheService, IMetricsService metrics, ILogger<ProjectCacheInvalidator> logger)
    {
        _cacheService = cacheService;
        _metrics = metrics;
        _logger = logger;
    }

    public async Task InvalidateAsync(Guid userId, Guid? projectId = null, CancellationToken cancellationToken = default)
    {
        var key = CacheKeys.UserProjects(userId);
        _logger.LogInformation("Invalidating cache key {CacheKey}", key);
        await _cacheService.RemoveAsync(key, cancellationToken);
        _metrics.IncrementCacheInvalidation();

        if (projectId is null)
            return;

        var key2 = CacheKeys.UserProject(userId, projectId.Value);
        var key3 = CacheKeys.ProjectTasks(userId, projectId.Value);
        _logger.LogInformation("Invalidating cache keys {CacheKey} and {CacheKeyTasks}", key2, key3);
        await _cacheService.RemoveAsync(key2, cancellationToken);
        await _cacheService.RemoveAsync(key3, cancellationToken);
        _metrics.IncrementCacheInvalidation();
    }
}
