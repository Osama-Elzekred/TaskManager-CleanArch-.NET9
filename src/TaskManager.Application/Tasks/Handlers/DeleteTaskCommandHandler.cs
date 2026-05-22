namespace TaskManager.Application.Tasks.Handlers;

using TaskManager.Application.Tasks.Commands;
using Common.Exceptions;
using Common.Interfaces;
using TaskManager.Application.Common;
using MediatR;

public class DeleteTaskCommandHandler : IRequestHandler<DeleteTaskCommand, Unit>
{
  private readonly IUnitOfWork _unitOfWork;
  private readonly ICurrentUserService _currentUserService;
  private readonly ProjectCacheInvalidator _cacheInvalidator;

  public DeleteTaskCommandHandler(
      IUnitOfWork unitOfWork,
      ICurrentUserService currentUserService,
      ProjectCacheInvalidator cacheInvalidator)
  {
    _unitOfWork = unitOfWork;
    _currentUserService = currentUserService;
    _cacheInvalidator = cacheInvalidator;
  }

  public async Task<Unit> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
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

    _unitOfWork.Tasks.Delete(task);
    await _unitOfWork.SaveChangesAsync(cancellationToken);
    await _cacheInvalidator.InvalidateAsync(_currentUserService.UserId, request.ProjectId, cancellationToken);

    return Unit.Value;
  }
}
