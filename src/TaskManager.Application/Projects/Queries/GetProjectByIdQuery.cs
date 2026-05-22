namespace TaskManager.Application.Projects.Queries;

using Dtos;
using MediatR;
using TaskManager.Application.Common;
using TaskManager.Application.Common.Interfaces;

public class GetProjectByIdQuery : IRequest<ProjectDto>, ICacheableRequest<ProjectDto>
{
  public string GetCacheKey(Guid userId) => CacheKeys.UserProject(userId, ProjectId);

  public Guid ProjectId { get; set; }
}
