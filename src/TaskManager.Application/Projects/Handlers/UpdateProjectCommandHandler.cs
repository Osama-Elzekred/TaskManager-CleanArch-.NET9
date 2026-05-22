namespace TaskManager.Application.Projects.Handlers;

using Commands;
using Common.Exceptions;
using Common.Interfaces;
using TaskManager.Application.Common;
using Dtos;
using MediatR;

public class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand, ProjectDto>
{
  private readonly IUnitOfWork _unitOfWork;
  private readonly ICurrentUserService _currentUserService;
  private readonly ProjectCacheInvalidator _cacheInvalidator;

  public UpdateProjectCommandHandler(
      IUnitOfWork unitOfWork,
      ICurrentUserService currentUserService,
      ProjectCacheInvalidator cacheInvalidator)
  {
    _unitOfWork = unitOfWork;
    _currentUserService = currentUserService;
    _cacheInvalidator = cacheInvalidator;
  }

  public async Task<ProjectDto> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
  {
    var project = await _unitOfWork.Projects.GetByIdAsync(request.ProjectId, cancellationToken);
    if (project == null || project.UserId != _currentUserService.UserId)
    {
      throw new NotFoundException("Project", request.ProjectId);
    }

    project.Name = request.Name;
    project.Description = request.Description;
    project.UpdatedAt = DateTime.UtcNow;

    _unitOfWork.Projects.Update(project);
    await _unitOfWork.SaveChangesAsync(cancellationToken);
    await _cacheInvalidator.InvalidateAsync(project.UserId, project.Id, cancellationToken);

    return new ProjectDto
    {
      Id = project.Id,
      Name = project.Name,
      Description = project.Description,
      UserId = project.UserId,
      CreatedAt = project.CreatedAt
    };
  }
}
