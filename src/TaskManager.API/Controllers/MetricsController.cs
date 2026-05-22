using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.Common.Interfaces;

namespace TaskManager.API.Controllers;

[ApiController]
[Route("/metrics")]
public class MetricsController : ControllerBase
{
  private readonly IMetricsService _metrics;

  public MetricsController(IMetricsService metrics)
  {
    _metrics = metrics;
  }

  [HttpGet]
  public IActionResult Get()
  {
    // Expose simple Prometheus-style metrics
    var lines = new List<string>
        {
            $"cache_hits_total {_metrics.GetCacheHits()}",
            $"cache_misses_total {_metrics.GetCacheMisses()}",
            $"cache_invalidations_total {_metrics.GetCacheInvalidations()}"
        };

    return Content(string.Join("\n", lines), "text/plain; version=0.0.4");
  }
}
