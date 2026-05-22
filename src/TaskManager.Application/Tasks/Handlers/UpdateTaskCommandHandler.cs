namespace TaskManager.Application.Tasks.Handlers;

using TaskManager.Application.Tasks.Commands;
using TaskManager.Application.Tasks.Dtos;
using Common.Exceptions;
using Common.Interfaces;
using TaskManager.Application.Common;
using MediatR;

public class UpdateTaskCommandHandler : IRequestHandler<UpdateTaskCommand, TaskDto>
{
  private readonly IUnitOfWork _unitOfWork;
  private readonly ICurrentUserService _currentUserService;
  private readonly ProjectCacheInvalidator _cacheInvalidator;

  public UpdateTaskCommandHandler(
      IUnitOfWork unitOfWork,
      ICurrentUserService currentUserService,
      ProjectCacheInvalidator cacheInvalidator)
  {
    _unitOfWork = unitOfWork;
    _currentUserService = currentUserService;
    _cacheInvalidator = cacheInvalidator;
  }

  public async Task<TaskDto> Handle(UpdateTaskCommand request, CancellationToken cancellationToken)
  {
    var project = await _unitOfWork.Projects.GetByIdAsync(request.ProjectId, cancellationToken);
    if (project == null || project.UserId != _currentUserService.UserId)
    {
      throw new NotFoundException("Project", request.ProjectId);
    }

    var task = await _unitOfWork.Tasks.GetByIdAsync(request.TaskId, cancellationToken);
    if (task == null || task.ProjectId != request.ProjectId || task.IsDeleted)
    {
      throw new NotFoundException("Task", request.TaskId);
    }

    task.Title = request.Title;
    task.Description = request.Description;
    task.Status = (Domain.Enums.TaskStatus)request.Status;
    task.DueDate = request.DueDate;
    task.Priority = (Domain.Enums.Priority)request.Priority;
    task.UpdatedAt = DateTime.UtcNow;

    _unitOfWork.Tasks.Update(task);
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
