namespace TaskManager.Application.Tasks.Handlers;

using TaskManager.Application.Tasks.Commands;
using TaskManager.Application.Tasks.Dtos;
using Common.Exceptions;
using Common.Interfaces;
using TaskManager.Application.Common;
using Domain.Entities;
using MediatR;

public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, TaskDto>
{
  private readonly IUnitOfWork _unitOfWork;
  private readonly ICurrentUserService _currentUserService;
  private readonly ProjectCacheInvalidator _cacheInvalidator;

  public CreateTaskCommandHandler(
      IUnitOfWork unitOfWork,
      ICurrentUserService currentUserService,
      ProjectCacheInvalidator cacheInvalidator)
  {
    _unitOfWork = unitOfWork;
    _currentUserService = currentUserService;
    _cacheInvalidator = cacheInvalidator;
  }

  public async Task<TaskDto> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
  {
    var project = await _unitOfWork.Projects.GetByIdAsync(request.ProjectId, cancellationToken);
    if (project == null || project.UserId != _currentUserService.UserId)
    {
      throw new NotFoundException("Project", request.ProjectId);
    }

    var task = new TaskItem
    {
      Id = Guid.NewGuid(),
      Title = request.Title,
      Description = request.Description,
      Status = (Domain.Enums.TaskStatus)request.Status,
      DueDate = request.DueDate,
      Priority = (Domain.Enums.Priority)request.Priority,
      ProjectId = request.ProjectId,
      CreatedAt = DateTime.UtcNow
    };

    await _unitOfWork.Tasks.AddAsync(task, cancellationToken);
    await _unitOfWork.SaveChangesAsync(cancellationToken);
    await _cacheInvalidator.InvalidateAsync(_currentUserService.UserId, request.ProjectId, cancellationToken);

    return new TaskDto
    {
      Id = task.Id,
      Title = task.Title,
      Description = task.Description,
      Status = (TaskStatus)task.Status,
      DueDate = task.DueDate,
      Priority = (Priority)task.Priority,
      ProjectId = task.ProjectId
    };
  }
}
