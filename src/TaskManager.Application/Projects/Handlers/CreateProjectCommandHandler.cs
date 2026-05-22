namespace TaskManager.Application.Projects.Handlers;

using Commands;
using Common.Interfaces;
using Dtos;
using Domain.Entities;
using TaskManager.Application.Common;
using MediatR;

public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, ProjectDto>
{
  private readonly IUnitOfWork _unitOfWork;
  private readonly ICurrentUserService _currentUserService;
  private readonly ProjectCacheInvalidator _cacheInvalidator;

  public CreateProjectCommandHandler(
      IUnitOfWork unitOfWork,
      ICurrentUserService currentUserService,
      ProjectCacheInvalidator cacheInvalidator)
  {
    _unitOfWork = unitOfWork;
    _currentUserService = currentUserService;
    _cacheInvalidator = cacheInvalidator;
  }

  public async Task<ProjectDto> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
  {
    var project = new Project
    {
      Id = Guid.NewGuid(),
      Name = request.Name,
      Description = request.Description,
      UserId = _currentUserService.UserId,
      CreatedAt = DateTime.UtcNow
    };

    await _unitOfWork.Projects.AddAsync(project, cancellationToken);
    await _unitOfWork.SaveChangesAsync(cancellationToken);
    await _cacheInvalidator.InvalidateAsync(_currentUserService.UserId, cancellationToken: cancellationToken);

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
