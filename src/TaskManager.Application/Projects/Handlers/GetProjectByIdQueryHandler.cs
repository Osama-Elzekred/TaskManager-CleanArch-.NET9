namespace TaskManager.Application.Projects.Handlers;

using Common.Exceptions;
using Common.Interfaces;
using Dtos;
using MediatR;
using Queries;
using TaskManager.Application.Common;

public class GetProjectByIdQueryHandler : IRequestHandler<GetProjectByIdQuery, ProjectDto>
{
  private readonly IUnitOfWork _unitOfWork;
  private readonly ICurrentUserService _currentUserService;

  public GetProjectByIdQueryHandler(
      IUnitOfWork unitOfWork,
      ICurrentUserService currentUserService)
  {
    _unitOfWork = unitOfWork;
    _currentUserService = currentUserService;
  }

  public async Task<ProjectDto> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
  {
    var project = await _unitOfWork.Projects.GetByIdAsync(request.ProjectId, cancellationToken);
    if (project == null || project.UserId != _currentUserService.UserId)
    {
      throw new NotFoundException("Project", request.ProjectId);
    }

    var dto = new ProjectDto
    {
      Id = project.Id,
      Name = project.Name,
      Description = project.Description,
      UserId = project.UserId,
      CreatedAt = project.CreatedAt
    };
    return dto;
  }
}
