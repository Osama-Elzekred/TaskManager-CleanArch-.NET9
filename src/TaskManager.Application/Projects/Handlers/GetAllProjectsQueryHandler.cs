namespace TaskManager.Application.Projects.Handlers;

using Common.Interfaces;
using Dtos;
using MediatR;
using Queries;

public class GetAllProjectsQueryHandler : IRequestHandler<GetAllProjectsQuery, List<ProjectDto>>
{
  private readonly IUnitOfWork _unitOfWork;
  private readonly ICurrentUserService _currentUserService;

  public GetAllProjectsQueryHandler(
      IUnitOfWork unitOfWork,
      ICurrentUserService currentUserService)
  {
    _unitOfWork = unitOfWork;
    _currentUserService = currentUserService;
  }

  public async Task<List<ProjectDto>> Handle(GetAllProjectsQuery request, CancellationToken cancellationToken)
  {
    var projects = await _unitOfWork.Projects.GetAllAsync(cancellationToken);
    var result = projects
        .Where(p => p.UserId == _currentUserService.UserId && !p.IsDeleted)
        .Select(p => new ProjectDto
        {
          Id = p.Id,
          Name = p.Name,
          Description = p.Description,
          UserId = p.UserId,
          CreatedAt = p.CreatedAt
        })
        .ToList();
    return result;
  }
}
