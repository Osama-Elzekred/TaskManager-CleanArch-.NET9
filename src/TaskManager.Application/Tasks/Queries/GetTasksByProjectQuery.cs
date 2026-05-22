namespace TaskManager.Application.Tasks.Queries;

using Dtos;
using MediatR;
using TaskManager.Application.Common;
using TaskManager.Application.Common.Interfaces;

public class GetTasksByProjectQuery : IRequest<List<TaskDto>>, ICacheableRequest<List<TaskDto>>
{
  public string GetCacheKey(Guid userId) => CacheKeys.ProjectTasks(userId, ProjectId);

  public Guid ProjectId { get; set; }
}
