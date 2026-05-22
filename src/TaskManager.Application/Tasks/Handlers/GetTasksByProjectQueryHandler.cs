namespace TaskManager.Application.Tasks.Handlers;

using Common.Exceptions;
using Common.Interfaces;
using TaskManager.Application.Tasks.Dtos;
using MediatR;
using TaskManager.Application.Tasks.Queries;
using TaskManager.Application.Common;

public class GetTasksByProjectQueryHandler : IRequestHandler<GetTasksByProjectQuery, List<TaskDto>>
{
  private readonly IUnitOfWork _unitOfWork;
  private readonly ICurrentUserService _currentUserService;

  public GetTasksByProjectQueryHandler(
      IUnitOfWork unitOfWork,
      ICurrentUserService currentUserService)
  {
    _unitOfWork = unitOfWork;
    _currentUserService = currentUserService;
  }

  public async Task<List<TaskDto>> Handle(GetTasksByProjectQuery request, CancellationToken cancellationToken)
  {
    var project = await _unitOfWork.Projects.GetByIdAsync(request.ProjectId, cancellationToken);
    if (project == null || project.UserId != _currentUserService.UserId)
    {
      throw new NotFoundException("Project", request.ProjectId);
    }

    var tasks = await _unitOfWork.Tasks.GetAllAsync(cancellationToken);
    var result = tasks
        .Where(t => t.ProjectId == request.ProjectId && !t.IsDeleted)
        .Select(t => new TaskDto
        {
          Id = t.Id,
          Title = t.Title,
          Description = t.Description,
          Status = (TaskStatus)t.Status,
          DueDate = t.DueDate,
          Priority = (Priority)t.Priority,
          ProjectId = t.ProjectId
        })
        .ToList();
    return result;
  }
}
