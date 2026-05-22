namespace TaskManager.Application.Projects.Queries;

using Dtos;
using MediatR;
using TaskManager.Application.Common;
using TaskManager.Application.Common.Interfaces;

public class GetAllProjectsQuery : IRequest<List<ProjectDto>>, ICacheableRequest<List<ProjectDto>>
{
  public string GetCacheKey(Guid userId) => CacheKeys.UserProjects(userId);
}
