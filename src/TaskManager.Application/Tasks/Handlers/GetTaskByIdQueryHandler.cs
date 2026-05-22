namespace TaskManager.Application.Tasks.Handlers;

using Common.Exceptions;
using Common.Interfaces;
using TaskManager.Application.Tasks.Dtos;
using MediatR;
using TaskManager.Application.Tasks.Queries;

public class GetTaskByIdQueryHandler : IRequestHandler<GetTaskByIdQuery, TaskDto>
{
  private readonly IUnitOfWork _unitOfWork;
  private readonly ICurrentUserService _currentUserService;

  public GetTaskByIdQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
  {
    _unitOfWork = unitOfWork;
    _currentUserService = currentUserService;
  }

  public async Task<TaskDto> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
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
